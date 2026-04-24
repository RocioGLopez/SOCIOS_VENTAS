using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrototipoCompras.Data;
using PrototipoCompras.Models;

namespace PrototipoCompras.Controllers;

[Authorize]
public class OrdenesController : Controller
{
    private readonly AppDbContext _db;

    public OrdenesController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var ordenes = await _db.OrdenesCompra
            .OrderByDescending(o => o.Id)
            .ToListAsync();

        return View(ordenes);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarcarRecibida(int id)
    {
        var orden = await _db.OrdenesCompra.FirstOrDefaultAsync(o => o.Id == id);

        if (orden == null)
            return RedirectToAction(nameof(Index));

        if (orden.Estado == "Recibida")
        {
            TempData["Error"] = "La orden ya fue recibida anteriormente.";
            return RedirectToAction(nameof(Index));
        }

        InventarioProducto? producto = null;

        if (orden.InventarioProductoId.HasValue)
        {
            producto = await _db.InventarioProductos
                .FirstOrDefaultAsync(p => p.Id == orden.InventarioProductoId.Value);
        }

        if (producto == null && !string.IsNullOrWhiteSpace(orden.Producto))
        {
            var nombreBuscado = orden.Producto.Trim().ToLower();

            producto = await _db.InventarioProductos
                .FirstOrDefaultAsync(p => p.Nombre.ToLower() == nombreBuscado);

            if (producto != null)
                orden.InventarioProductoId = producto.Id;
        }

        if (producto == null)
        {
            TempData["Error"] = "No se encontró el producto en inventario para actualizar stock.";
            return RedirectToAction(nameof(Index));
        }

        // Historia 6
        orden.Estado = "Recibida";
        orden.FechaRecepcion = DateTime.UtcNow;
        orden.UsuarioRecepcion = User?.Identity?.Name ?? "Sistema";

        producto.Stock += orden.Cantidad;
        producto.UltimaActualizacion = DateTime.UtcNow;

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

        await _db.SaveChangesAsync();

        TempData["Ok"] = "La orden fue marcada como recibida y el inventario se actualizó correctamente.";
        return RedirectToAction(nameof(Index));
    }
}