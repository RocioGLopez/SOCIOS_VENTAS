using System.ComponentModel.DataAnnotations;

namespace PrototipoCompras.Models;

public class InventarioProducto
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Nombre { get; set; } = "";

    [Required, StringLength(150)]
    public string Proveedor { get; set; } = "";

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    [Range(0, int.MaxValue)]
    public int StockMinimo { get; set; }

    public DateTime UltimaActualizacion { get; set; } = DateTime.UtcNow;
}