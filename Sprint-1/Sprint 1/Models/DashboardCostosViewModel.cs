namespace PrototipoCompras.Models;

public class DashboardCostosViewModel
{
    public string? ProductoSeleccionado { get; set; }
    public List<string> ProductosDisponibles { get; set; } = new();
    public List<string> ProveedoresDisponibles { get; set; } = new();
    public List<string> ProveedoresSeleccionados { get; set; } = new();

    public List<ComparativoCostoProveedorViewModel> Comparativos { get; set; } = new();

    public string? MejorProveedor { get; set; }
    public decimal MejorPrecio { get; set; }
    public decimal PromedioGeneralHistorico { get; set; }
    public decimal DiferenciaMaxMinActual { get; set; }

    public int TiempoMs { get; set; }
    public string? Error { get; set; }
}