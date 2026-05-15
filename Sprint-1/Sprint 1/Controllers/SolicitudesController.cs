using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrototipoCompras.Data;
using PrototipoCompras.Models;
using PrototipoCompras.Services;

namespace PrototipoCompras.Controllers;

public class SolicitudesController : Controller
{
    private readonly AppDbContext _db;
    private readonly IBitacoraService _bitacora;

    public SolicitudesController(AppDbContext db, IBitacoraService bitacora)
    {
        _db = db;
        _bitacora = bitacora;
    }

    public IActionResult Create()
    {
        return View(new SolicitudCompra { Fecha = DateTime.Today });
    }

    [HttpPost]
    public async Task<IActionResult> Create(SolicitudCompra model)
    {
        var start = DateTime.UtcNow;

        if (!ModelState.IsValid)
            return View(model);

        model.Estado = "Pendiente de aprobación";

        if (string.IsNullOrWhiteSpace(model.Solicitante))
            model.Solicitante = "EmpleadoDemo";

        model.GeneradaAutomaticamente = false;

        var productoInventario = await BuscarProductoInventarioAsync(model.Producto);
        if (productoInventario != null)
            model.InventarioProductoId = productoInventario.Id;

        _db.Solicitudes.Add(model);
        await _db.SaveChangesAsync();

        await _bitacora.RegistrarAsync(
            "Solicitudes",
            "Crear solicitud",
            $"Producto: {model.Producto}, cantidad: {model.Cantidad}, solicitante: {model.Solicitante}",
            model.Id);

        ViewBag.TiempoMs = (int)Math.Round((DateTime.UtcNow - start).TotalMilliseconds);
        return View("Success", model);
    }

    public IActionResult BossPanel()
    {
        var list = _db.Solicitudes
            .Where(s => s.Estado == "Pendiente" || s.Estado == "Pendiente de aprobación")
            .OrderByDescending(s => s.Id)
            .ToList();

        return View(list);
    }

    public async Task<IActionResult> Approve(int id)
    {
        var s = await _db.Solicitudes.FirstOrDefaultAsync(x => x.Id == id);

        if (s == null)
            return RedirectToAction(nameof(BossPanel));

        if (s.Estado == "Aprobada")
            return RedirectToAction(nameof(BossPanel));

        s.Estado = "Aprobada";

        InventarioProducto? producto = null;

        if (s.InventarioProductoId.HasValue)
        {
            producto = await _db.InventarioProductos
                .FirstOrDefaultAsync(p => p.Id == s.InventarioProductoId.Value);
        }

        if (producto == null)
        {
            producto = await BuscarProductoInventarioAsync(s.Producto);

            if (producto != null && !s.InventarioProductoId.HasValue)
                s.InventarioProductoId = producto.Id;
        }

        var existeOrden = await _db.OrdenesCompra
            .AnyAsync(o => o.SolicitudCompraId == s.Id);

        if (!existeOrden)
        {
            _db.OrdenesCompra.Add(new OrdenCompra
            {
                NumeroOrden = $"OC-{DateTime.UtcNow:yyyyMMddHHmmss}-{s.Id}",
                Producto = s.Producto,
                Cantidad = s.Cantidad,
                Proveedor = producto?.Proveedor ?? "Proveedor pendiente",
                Estado = "Emitida",
                FechaCreacion = DateTime.UtcNow,
                InventarioProductoId = producto?.Id,
                SolicitudCompraId = s.Id
            });
        }

        await _db.SaveChangesAsync();

        await _bitacora.RegistrarAsync(
            "Solicitudes",
            "Aprobar solicitud",
            $"Solicitud #{s.Id} aprobada. Producto: {s.Producto}.",
            s.Id);

        return RedirectToAction(nameof(BossPanel));
    }

    public async Task<IActionResult> Reject(int id)
    {
        var s = await _db.Solicitudes.FirstOrDefaultAsync(x => x.Id == id);

        if (s == null)
            return RedirectToAction(nameof(BossPanel));

        if (s.Estado == "Rechazada")
            return RedirectToAction(nameof(BossPanel));

        s.Estado = "Rechazada";

        await _db.SaveChangesAsync();

        await _bitacora.RegistrarAsync(
            "Solicitudes",
            "Rechazar solicitud",
            $"Solicitud #{s.Id} rechazada. Producto: {s.Producto}.",
            s.Id);

        return RedirectToAction(nameof(BossPanel));
    }

    private async Task<InventarioProducto?> BuscarProductoInventarioAsync(string nombreProducto)
    {
        if (string.IsNullOrWhiteSpace(nombreProducto))
            return null;

        var nombreBuscado = nombreProducto.Trim().ToLower();

        return await _db.InventarioProductos
            .FirstOrDefaultAsync(p => p.Nombre.ToLower() == nombreBuscado);
    }
}