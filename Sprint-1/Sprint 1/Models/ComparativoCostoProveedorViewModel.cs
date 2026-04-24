namespace PrototipoCompras.Models;

public class ComparativoCostoProveedorViewModel
{
    public string Proveedor { get; set; } = "";
    public decimal PrecioActual { get; set; }
    public decimal PromedioHistorico { get; set; }
    public decimal DiferenciaVsPromedio { get; set; }
    public DateTime UltimaFecha { get; set; }
}