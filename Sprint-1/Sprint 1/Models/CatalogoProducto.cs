using System.ComponentModel.DataAnnotations;

namespace PrototipoCompras.Models;

/// <summary>
/// Catálogo de productos disponibles para solicitudes externas.
/// Incluye los 5 productos base + los que se agreguen dinámicamente.
/// </summary>
public class CatalogoProducto
{
    public int    Id              { get; set; }

    [Required, MaxLength(150)]
    public string Nombre          { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Categoria       { get; set; } = "General";

    public int    StockDisponible { get; set; }
    public int    CantidadMinima  { get; set; } = 1;
    public int    CantidadMaxima  { get; set; } = 25;
    public bool   Activo          { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}
