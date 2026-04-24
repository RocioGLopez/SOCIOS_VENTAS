using System.ComponentModel.DataAnnotations;

namespace PrototipoCompras.Models;

public class CostoProveedorHistorico
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Producto { get; set; } = "";

    [Required, StringLength(150)]
    public string Proveedor { get; set; } = "";

    [Range(typeof(decimal), "0.01", "999999999")]
    public decimal PrecioUnitario { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
}