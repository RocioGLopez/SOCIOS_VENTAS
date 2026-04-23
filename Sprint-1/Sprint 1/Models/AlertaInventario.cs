using System.ComponentModel.DataAnnotations;

namespace PrototipoCompras.Models;

public class AlertaInventario
{
    public int Id { get; set; }

    public int InventarioProductoId { get; set; }

    [Required, StringLength(200)]
    public string Producto { get; set; } = "";

    public int StockActual { get; set; }

    public int StockMinimo { get; set; }

    [Required, StringLength(500)]
    public string Mensaje { get; set; } = "";

    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    public bool Resuelta { get; set; } = false;
}