using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrototipoCompras.Data;
using PrototipoCompras.Models;

namespace PrototipoCompras.Controllers;

public class SolicitudesController : Controller
{
    private readonly AppDbContext _db;
    public SolicitudesController(AppDbContext db) => _db = db;

    // GET: /Solicitudes/Create
    public IActionResult Create()
    {
        return View(new SolicitudCompra { Fecha = DateTime.Today });
    }

    // POST: /Solicitudes/Create
    [HttpPost]
    public async Task<IActionResult> Create(SolicitudCompra model)
    {
        var start = DateTime.UtcNow;

        if (!ModelState.IsValid)
            return View(model);

        model.Estado = "Pendiente de aprobación";

        if (string.IsNullOrWhiteSpace(model.Solicitante))
            model.Solicitante = "EmpleadoDemo";

        _db.Solicitudes.Add(model);
        await _db.SaveChangesAsync();

        ViewBag.TiempoMs = (int)Math.Round((DateTime.UtcNow - start).TotalMilliseconds);
        return View("Success", model);
    }

    // Panel del jefe: mostrar solo solicitudes pendientes
    public IActionResult BossPanel()
    {
        var list = _db.Solicitudes
            .Where(s => s.Estado == "Pendiente" || s.Estado == "Pendiente de aprobación")
            .OrderByDescending(s => s.Id)
            .ToList();

        return View(list);
    }

    // Aprobar solicitud
    public async Task<IActionResult> Approve(int id)
    {
        var s = await _db.Solicitudes.FirstOrDefaultAsync(x => x.Id == id);

        if (s == null)
            return RedirectToAction(nameof(BossPanel));

        if (s.Estado == "Aprobada")
            return RedirectToAction(nameof(BossPanel));

        s.Estado = "Aprobada";

        // Si la solicitud fue generada automáticamente por inventario,
        // al aprobarla también se repone el stock.
        if (s.GeneradaAutomaticamente && s.InventarioProductoId.HasValue)
        {
            var producto = await _db.InventarioProductos
                .FirstOrDefaultAsync(p => p.Id == s.InventarioProductoId.Value);

            if (producto != null)
            {
                producto.Stock += s.Cantidad;
                producto.UltimaActualizacion = DateTime.UtcNow;

                // Si el stock ya quedó normal, cerrar alertas y notificaciones
                if (producto.Stock >= producto.StockMinimo)
                {
                    var alertas = await _db.AlertasInventario
                        .Where(a => a.InventarioProductoId == producto.Id && !a.Resuelta)
                        .ToListAsync();

                    foreach (var alerta in alertas)
                        alerta.Resuelta = true;

                    var notificaciones = await _db.NotificacionesCompra
                        .Where(n => n.InventarioProductoId == producto.Id && !n.Leida)
                        .ToListAsync();

                    foreach (var notificacion in notificaciones)
                        notificacion.Leida = true;
                }
            }
        }

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(BossPanel));
    }

    // Rechazar solicitud
    public async Task<IActionResult> Reject(int id)
    {
        var s = await _db.Solicitudes.FirstOrDefaultAsync(x => x.Id == id);

        if (s == null)
            return RedirectToAction(nameof(BossPanel));

        if (s.Estado == "Rechazada")
            return RedirectToAction(nameof(BossPanel));

        s.Estado = "Rechazada";

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(BossPanel));
    }
}