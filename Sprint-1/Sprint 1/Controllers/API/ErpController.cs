using Microsoft.AspNetCore.Mvc;

namespace PrototipoCompras.Controllers.Api;

[ApiController]
[Route("api/erp")]
[Produces("application/json")]
public class ErpController : ControllerBase
{
    // ── GET /api/erp/compras/historial ─────────────────────────
    [HttpGet("compras/historial")]
    public IActionResult HistorialCompras([FromQuery] string? inicio, [FromQuery] string? fin)
    {
        var compras = new[]
        {
            new { id = 1, fecha = "2026-03-01", proveedor = "Proveedor A", monto = 1500, estado = "Pagado" },
            new { id = 2, fecha = "2026-03-10", proveedor = "Proveedor B", monto = 2500, estado = "Pendiente" },
            new { id = 3, fecha = "2026-03-15", proveedor = "Proveedor C", monto = 870,  estado = "Cancelado" },
            new { id = 4, fecha = "2026-03-20", proveedor = "Proveedor A", monto = 4100, estado = "Pagado" },
            new { id = 5, fecha = "2026-03-25", proveedor = "Proveedor D", monto = 620,  estado = "Pendiente" },
        }.AsEnumerable();

        if (!string.IsNullOrEmpty(inicio))
            compras = compras.Where(c => string.Compare(c.fecha, inicio) >= 0);
        if (!string.IsNullOrEmpty(fin))
            compras = compras.Where(c => string.Compare(c.fecha, fin) <= 0);

        return Ok(compras);
    }

    // ── GET /api/erp/contratos ─────────────────────────────────
    [HttpGet("contratos")]
    public IActionResult Contratos([FromQuery] string? proveedor)
    {
        var contratos = new[]
        {
            new { id = 1, numero = "CON-001", proveedor = "Proveedor A", inicio = "2026-01-01", vencimiento = "2026-04-01", estado = "Activo",    documento = "contrato1.pdf" },
            new { id = 2, numero = "CON-002", proveedor = "Proveedor B", inicio = "2026-02-01", vencimiento = "2026-03-28", estado = "Por vencer", documento = "contrato2.pdf" },
            new { id = 3, numero = "CON-003", proveedor = "Proveedor C", inicio = "2025-06-01", vencimiento = "2026-06-01", estado = "Activo",    documento = "contrato3.pdf" },
            new { id = 4, numero = "CON-004", proveedor = "Proveedor D", inicio = "2025-01-01", vencimiento = "2026-02-01", estado = "Vencido",   documento = "contrato4.pdf" },
        }.AsEnumerable();

        if (!string.IsNullOrEmpty(proveedor))
            contratos = contratos.Where(c => c.proveedor.ToLower().Contains(proveedor.ToLower()));

        return Ok(contratos);
    }

    // ── GET /api/erp/inventario ────────────────────────────────
    [HttpGet("inventario")]
    public IActionResult Inventario()
    {
        var inventario = new[]
        {
            new { id = 1, nombre = "Laptop Dell",      proveedor = "TechSupply S.A.",       stock = 10, stock_minimo = 5, ultima_actualizacion = "2026-04-23" },
            new { id = 2, nombre = "Sillas de oficina",proveedor = "OfiMundo",              stock = 8,  stock_minimo = 4, ultima_actualizacion = "2026-04-23" },
            new { id = 3, nombre = "Monitor LG",       proveedor = "Distribuidora Central", stock = 6,  stock_minimo = 3, ultima_actualizacion = "2026-04-23" },
        };
        return Ok(inventario);
    }

    // ── GET /api/erp/alertas-inventario ───────────────────────
    [HttpGet("alertas-inventario")]
    public IActionResult AlertasInventario()
    {
        return Ok(new object[] { });
    }

    // ── GET /api/erp/solicitudes-compra ───────────────────────
    [HttpGet("solicitudes-compra")]
    public IActionResult SolicitudesCompra()
    {
        return Ok(new object[] { });
    }

    // ── GET /api/erp/notificaciones/jefe-compras ──────────────
    [HttpGet("notificaciones/jefe-compras")]
    public IActionResult NotificacionesJefeCompras()
    {
        return Ok(new object[] { });
    }

    // ── POST /api/erp/inventario/ajustar-stock ─────────────────
    [HttpPost("inventario/ajustar-stock")]
    public IActionResult AjustarStock([FromBody] AjusteStockDto dto)
    {
        return Ok(new { mensaje = "Stock actualizado correctamente" });
    }

    // ── POST /api/erp/inventario/configurar-minimo ─────────────
    [HttpPost("inventario/configurar-minimo")]
    public IActionResult ConfigurarMinimo([FromBody] ConfigurarMinimoDto dto)
    {
        return Ok(new { mensaje = "Nivel mínimo actualizado correctamente" });
    }
}

public class AjusteStockDto
{
    public int ProductoId { get; set; }
    public int Stock { get; set; }
}

public class ConfigurarMinimoDto
{
    public int ProductoId { get; set; }
    public int StockMinimo { get; set; }
}