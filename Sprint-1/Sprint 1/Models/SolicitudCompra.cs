using System.ComponentModel.DataAnnotations;

namespace PrototipoCompras.Models;

public class SolicitudCompra
{
    public int Id { get; set; }

    [Required]
    public DateTime Fecha { get; set; }

    [Required, StringLength(200)]
    public string Producto { get; set; } = "";

    [Range(1, int.MaxValue)]
    public int Cantidad { get; set; }

    [Required, StringLength(500)]
    public string Justificacion { get; set; } = "";

    [Required]
    public string Estado { get; set; } = "Pendiente de aprobación";

    [Required]
    public string Solicitante { get; set; } = "";

    public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
}