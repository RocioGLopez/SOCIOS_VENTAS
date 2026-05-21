using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrototipoCompras.Data;
using PrototipoCompras.Models;
using PrototipoCompras.Services;

namespace PrototipoCompras.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SolicitudesApiController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IBitacoraService _bitacora;

    public SolicitudesApiController(AppDbContext db, IBitacoraService bitacora)
    {
        _db = db;
        _bitacora = bitacora;
    }

    // ─────────────────────────────────────────────
    // GET /api/solicitudesapi/catalogo
    // Devuelve el catálogo de productos disponibles
    // ─────────────────────────────────────────────
    [HttpGet("catalogo")]
    public async Task<IActionResult> GetCatalogo()
    {
        var productosDB = await _db.CatalogoProductos
            .Where(p => p.Activo)
            .OrderBy(p => p.Nombre)
            .Select(p => new CatalogoProductoDto
            {
                Id       = p.Id,
                Nombre   = p.Nombre,
                Categoria = p.Categoria,
                StockDisponible = p.StockDisponible,
                CantidadMinima  = p.CantidadMinima,
                CantidadMaxima  = p.CantidadMaxima
            })
            .ToListAsync();

        return Ok(new
        {
            total    = productosDB.Count,
            productos = productosDB
        });
    }

    // ─────────────────────────────────────────────
    // POST /api/solicitudesapi/catalogo
    // Agrega un nuevo producto al catálogo
    // ─────────────────────────────────────────────
    [HttpPost("catalogo")]
    public async Task<IActionResult> AgregarProductoCatalogo([FromBody] NuevoProductoCatalogoDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existe = await _db.CatalogoProductos
            .AnyAsync(p => p.Nombre.ToLower() == dto.Nombre.Trim().ToLower());

        if (existe)
            return Conflict(new { mensaje = $"El producto '{dto.Nombre}' ya existe en el catálogo." });

        var producto = new CatalogoProducto
        {
            Nombre          = dto.Nombre.Trim(),
            Categoria       = dto.Categoria?.Trim() ?? "General",
            StockDisponible = dto.StockDisponible,
            CantidadMinima  = dto.CantidadMinima > 0 ? dto.CantidadMinima : 1,
            CantidadMaxima  = dto.CantidadMaxima > 0 ? dto.CantidadMaxima : 100,
            Activo          = true,
            FechaCreacion   = DateTime.UtcNow
        };

        _db.CatalogoProductos.Add(producto);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCatalogo), new { id = producto.Id }, new
        {
            mensaje  = "Producto agregado al catálogo exitosamente.",
            producto = new CatalogoProductoDto
            {
                Id              = producto.Id,
                Nombre          = producto.Nombre,
                Categoria       = producto.Categoria,
                StockDisponible = producto.StockDisponible,
                CantidadMinima  = producto.CantidadMinima,
                CantidadMaxima  = producto.CantidadMaxima
            }
        });
    }

    // ─────────────────────────────────────────────
    // POST /api/solicitudesapi
    // Crea una solicitud de compra externa (multi-producto)
    // ─────────────────────────────────────────────
    [HttpPost]
    public async Task<IActionResult> CrearSolicitud([FromBody] SolicitudExternaDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (dto.Productos == null || !dto.Productos.Any())
            return BadRequest(new { mensaje = "Debe incluir al menos un producto en la solicitud." });

        if (dto.FechaEntregaRequerida < DateTime.Today)
            return BadRequest(new { mensaje = "La fecha de entrega no puede ser una fecha pasada." });

        var errores = new List<string>();
        var solicitudesCreadas = new List<object>();

        foreach (var item in dto.Productos)
        {
            // Buscar el producto en el catálogo
            var productoCatalogo = await _db.CatalogoProductos
                .FirstOrDefaultAsync(p => p.Id == item.ProductoId && p.Activo);

            if (productoCatalogo == null)
            {
                errores.Add($"Producto con Id {item.ProductoId} no encontrado en el catálogo.");
                continue;
            }

            if (item.Cantidad < productoCatalogo.CantidadMinima || item.Cantidad > productoCatalogo.CantidadMaxima)
            {
                errores.Add($"La cantidad para '{productoCatalogo.Nombre}' debe estar entre " +
                            $"{productoCatalogo.CantidadMinima} y {productoCatalogo.CantidadMaxima}.");
                continue;
            }

            // Buscar producto en inventario
            var productoInventario = await _db.InventarioProductos
                .FirstOrDefaultAsync(p => p.Nombre.ToLower() == productoCatalogo.Nombre.ToLower());

            var solicitud = new SolicitudCompra
            {
                Fecha        = DateTime.Today,
                Producto     = productoCatalogo.Nombre,
                Cantidad     = item.Cantidad,
                Justificacion = dto.Justificacion ?? $"Solicitud externa de {dto.NombreEntidad}",
                Solicitante  = dto.NombreEntidad,
                Estado       = "Pendiente de aprobación",
                GeneradaAutomaticamente = false,
                FechaEntregaRequerida   = dto.FechaEntregaRequerida,
                InventarioProductoId    = productoInventario?.Id
            };

            _db.Solicitudes.Add(solicitud);
            await _db.SaveChangesAsync();

            await _bitacora.RegistrarAsync(
                "SolicitudesApi",
                "Crear solicitud externa",
                $"Entidad: {dto.NombreEntidad}, Producto: {productoCatalogo.Nombre}, Cantidad: {item.Cantidad}",
                solicitud.Id);

            solicitudesCreadas.Add(new
            {
                solicitudId = solicitud.Id,
                producto    = solicitud.Producto,
                cantidad    = solicitud.Cantidad,
                estado      = solicitud.Estado,
                fechaEntregaRequerida = solicitud.FechaEntregaRequerida
            });
        }

        if (!solicitudesCreadas.Any())
            return BadRequest(new { mensaje = "No se pudo crear ninguna solicitud.", errores });

        return Ok(new
        {
            mensaje   = $"Se crearon {solicitudesCreadas.Count} solicitud(es) exitosamente.",
            solicitudes = solicitudesCreadas,
            errores   = errores.Any() ? errores : null
        });
    }

    // ─────────────────────────────────────────────
    // GET /api/solicitudesapi/{id}/estado
    // Consulta el estado de una solicitud
    // ─────────────────────────────────────────────
    [HttpGet("{id}/estado")]
    public async Task<IActionResult> GetEstado(int id)
    {
        var s = await _db.Solicitudes.FirstOrDefaultAsync(x => x.Id == id);

        if (s == null)
            return NotFound(new { mensaje = $"Solicitud #{id} no encontrada." });

        return Ok(new
        {
            solicitudId           = s.Id,
            producto              = s.Producto,
            cantidad              = s.Cantidad,
            estado                = s.Estado,
            solicitante           = s.Solicitante,
            fecha                 = s.Fecha,
            fechaEntregaRequerida = s.FechaEntregaRequerida
        });
    }
}

// ─────────────────────────────────────────────
// DTOs
// ─────────────────────────────────────────────

public class SolicitudExternaDto
{
    /// <summary>Nombre de la entidad o empresa que realiza la solicitud</summary>
    public required string NombreEntidad { get; set; }

    /// <summary>Justificación general de la solicitud</summary>
    public string? Justificacion { get; set; }

    /// <summary>Fecha en que se requieren los productos</summary>
    public DateTime FechaEntregaRequerida { get; set; }

    /// <summary>Lista de productos con sus cantidades</summary>
    public List<ItemSolicitudDto> Productos { get; set; } = new();
}

public class ItemSolicitudDto
{
    /// <summary>Id del producto del catálogo</summary>
    public int ProductoId { get; set; }

    /// <summary>Cantidad solicitada</summary>
    public int Cantidad { get; set; }
}

public class CatalogoProductoDto
{
    public int    Id              { get; set; }
    public string Nombre          { get; set; } = string.Empty;
    public string Categoria       { get; set; } = string.Empty;
    public int    StockDisponible { get; set; }
    public int    CantidadMinima  { get; set; }
    public int    CantidadMaxima  { get; set; }
}

public class NuevoProductoCatalogoDto
{
    public required string Nombre          { get; set; }
    public string?         Categoria       { get; set; }
    public int             StockDisponible { get; set; }
    public int             CantidadMinima  { get; set; } = 1;
    public int             CantidadMaxima  { get; set; } = 100;
}
