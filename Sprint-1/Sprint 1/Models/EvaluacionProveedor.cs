using System.ComponentModel.DataAnnotations;

namespace PrototipoCompras.Models;

public class EvaluacionProveedor
{
    public int Id { get; set; }

    [Required]
    public int ProveedorId { get; set; }
    public Proveedor? Proveedor { get; set; }

    [Range(1,5)]
    public int Score { get; set; }

    [StringLength(500)]
    public string? Comentario { get; set; }

    [Required]
    public string Evaluador { get; set; } = "";

    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}