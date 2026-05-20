using System.Text.Json.Serialization;

namespace PrototipoCompras.Models;

public class ContactoExternoDto
{
    [JsonPropertyName("idContacto")]
    public int IdContacto { get; set; }

    [JsonPropertyName("nombre")]
    public string Nombre { get; set; } = "";

    [JsonPropertyName("apellido")]
    public string? Apellido { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; } = "";

    [JsonPropertyName("telefono")]
    public string? Telefono { get; set; }

    [JsonPropertyName("empresa")]
    public EmpresaExternaDto? Empresa { get; set; }

    [JsonIgnore]
    public string NombreCompleto =>
        string.IsNullOrWhiteSpace(Apellido)
            ? Nombre
            : $"{Nombre} {Apellido}".Trim();

    [JsonIgnore]
    public string EmpresaNombre =>
        Empresa?.Nombre ?? "-";
}