using System.ComponentModel.DataAnnotations;

namespace PrototipoCompras.Models;

public class Bitacora
{
    public int Id { get; set; }

    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    [Required, StringLength(150)]
    public string Usuario { get; set; } = "";

    [Required, StringLength(80)]
    public string Modulo { get; set; } = "";

    [Required, StringLength(120)]
    public string Accion { get; set; } = "";

    [StringLength(1000)]
    public string? Detalle { get; set; }

    public int? EntidadId { get; set; }

    [StringLength(45)]
    public string? IpAddress { get; set; }

    public bool Exitoso { get; set; } = true;
}
