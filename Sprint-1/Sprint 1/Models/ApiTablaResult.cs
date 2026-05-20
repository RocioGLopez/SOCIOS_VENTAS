namespace PrototipoCompras.Models;

public class ApiTablaResult
{
    public string Titulo { get; set; } = "";
    public string Endpoint { get; set; } = "";
    public List<string> Columnas { get; set; } = new();
    public List<Dictionary<string, string>> Filas { get; set; } = new();
    public string? Error { get; set; }
    public string? RawJson { get; set; }
}