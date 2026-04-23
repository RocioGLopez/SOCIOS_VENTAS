using System.ComponentModel.DataAnnotations;

namespace PrototipoCompras.Models;

public class NotificacionCompra
{
    public int Id { get; set; }

    public int? InventarioProductoId { get; set; }

    [Required, StringLength(100)]
    public string Destinatario { get; set; } = "Jefe de Compras";

    [Required, StringLength(500)]
    public string Mensaje { get; set; } = "";

    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    public bool Leida { get; set; } = false;
}