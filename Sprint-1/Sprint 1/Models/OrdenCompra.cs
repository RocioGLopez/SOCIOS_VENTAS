using System.ComponentModel.DataAnnotations;

namespace PrototipoCompras.Models;

public class OrdenCompra
{
    public int Id { get; set; }

    [Required, StringLength(30)]
    public string NumeroOrden { get; set; } = "";

    [Required, StringLength(200)]
    public string Producto { get; set; } = "";

    [Range(1, int.MaxValue)]
    public int Cantidad { get; set; }

    [Required, StringLength(150)]
    public string Proveedor { get; set; } = "";

    [Required, StringLength(50)]
    public string Estado { get; set; } = "Emitida";

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public DateTime? FechaRecepcion { get; set; }

    [StringLength(200)]
    public string? UsuarioRecepcion { get; set; }

    public int? InventarioProductoId { get; set; }

    public int? SolicitudCompraId { get; set; }
}