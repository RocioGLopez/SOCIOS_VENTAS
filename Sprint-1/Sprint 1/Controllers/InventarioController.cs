using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrototipoCompras.Data;
using PrototipoCompras.Models;
using PrototipoCompras.Services;

namespace PrototipoCompras.Controllers;

[Authorize]
public class InventarioController : Controller
{
    private readonly AppDbContext _db;
    private readonly IBitacoraService _bitacora;

    public InventarioController(AppDbContext db, IBitacoraService bitacora)
    {
        _db = db;
        _bitacora = bitacora;
    }

    public async Task<IActionResult> Index()
    {
        var productos = await _db.InventarioProductos
            .OrderBy(p => p.Nombre)
            .ToListAsync();

        foreach (var producto in productos)
        {
            await VerificarStockBajoAsync(producto);
        }

        await _db.SaveChangesAsync();

        var model = new InventarioIndexViewModel
        {
            Productos = productos,
            Alertas = await _db.AlertasInventario
                .Where(a => !a.Resuelta)
                .OrderByDescending(a => a.Fecha)
                .ToListAsync(),

            SolicitudesPendientes = await _db.Solicitudes
                .Where(s => s.GeneradaAutomaticamente && s.Estado == "Pendiente")
                .OrderByDescending(s => s.Id)
                .ToListAsync(),

            Notificaciones = await _db.NotificacionesCompra
                .OrderByDescending(n => n.Fecha)
                .ToListAsync()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ActualizarStock(int id, int stock)
    {
        var producto = await _db.InventarioProductos.FindAsync(id);
        if (producto == null)
            return NotFound();

        if (stock < 0)
        {
            TempData["Error"] = "El stock no puede ser negativo.";
            return RedirectToAction(nameof(Index));
        }

        producto.Stock = stock;
        producto.UltimaActualizacion = DateTime.UtcNow;

        await VerificarStockBajoAsync(producto);
        await _db.SaveChangesAsync();

        await _bitacora.RegistrarAsync(
            "Inventario",
            "Actualizar stock",
            $"Producto {producto.Nombre} (Id {producto.Id}): stock = {stock}.",
            producto.Id);

        TempData["Ok"] = "Stock actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfigurarMinimo(int id, int stockMinimo)
    {
        var producto = await _db.InventarioProductos.FindAsync(id);
        if (producto == null)
            return NotFound();

        if (stockMinimo < 0)
        {
            TempData["Error"] = "El nivel mínimo no puede ser negativo.";
            return RedirectToAction(nameof(Index));
        }

        producto.StockMinimo = stockMinimo;
        producto.UltimaActualizacion = DateTime.UtcNow;

        await VerificarStockBajoAsync(producto);
        await _db.SaveChangesAsync();

        await _bitacora.RegistrarAsync(
            "Inventario",
            "Configurar stock mínimo",
            $"Producto {producto.Nombre} (Id {producto.Id}): mínimo = {stockMinimo}.",
            producto.Id);

        TempData["Ok"] = "Nivel mínimo actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task VerificarStockBajoAsync(InventarioProducto producto)
    {
        if (producto.Stock >= producto.StockMinimo)
        {
            var alertasAbiertas = await _db.AlertasInventario
                .Where(a => a.InventarioProductoId == producto.Id && !a.Resuelta)
                .ToListAsync();

            foreach (var alerta in alertasAbiertas)
                alerta.Resuelta = true;

            var notificacionesAbiertas = await _db.NotificacionesCompra
                .Where(n => n.InventarioProductoId == producto.Id && !n.Leida)
                .ToListAsync();

            foreach (var notificacion in notificacionesAbiertas)
                notificacion.Leida = true;

            return;
        }

        var existeAlerta = await _db.AlertasInventario
            .AnyAsync(a => a.InventarioProductoId == producto.Id && !a.Resuelta);

        if (!existeAlerta)
        {
            _db.AlertasInventario.Add(new AlertaInventario
            {
                InventarioProductoId = producto.Id,
                Producto = producto.Nombre,
                StockActual = producto.Stock,
                StockMinimo = producto.StockMinimo,
                Mensaje = $"El producto {producto.Nombre} tiene stock {producto.Stock}, por debajo del mínimo {producto.StockMinimo}.",
                Fecha = DateTime.UtcNow,
                Resuelta = false
            });
        }

        var existeSolicitudPendiente = await _db.Solicitudes
            .AnyAsync(s => s.InventarioProductoId == producto.Id
                        && s.GeneradaAutomaticamente
                        && s.Estado == "Pendiente");

        if (!existeSolicitudPendiente)
        {
            var cantidadSugerida = Math.Max((producto.StockMinimo * 2) - producto.Stock, 1);

            _db.Solicitudes.Add(new SolicitudCompra
            {
                Fecha = DateTime.Today,
                Producto = producto.Nombre,
                Cantidad = cantidadSugerida,
                Justificacion = "Generada automáticamente por stock bajo.",
                Estado = "Pendiente",
                Solicitante = "Sistema",
                CreadoEn = DateTime.UtcNow,
                GeneradaAutomaticamente = true,
                InventarioProductoId = producto.Id
            });
        }

        var existeNotificacion = await _db.NotificacionesCompra
            .AnyAsync(n => n.InventarioProductoId == producto.Id && !n.Leida);

        if (!existeNotificacion)
        {
            _db.NotificacionesCompra.Add(new NotificacionCompra
            {
                InventarioProductoId = producto.Id,
                Destinatario = "Jefe de Compras",
                Mensaje = $"Se generó una solicitud de compra pendiente para {producto.Nombre} por stock bajo.",
                Fecha = DateTime.UtcNow,
                Leida = false
            });
        }
    }
}