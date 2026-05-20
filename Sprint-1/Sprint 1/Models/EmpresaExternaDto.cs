using System.Text.Json.Serialization;

namespace PrototipoCompras.Models;

public class EmpresaExternaDto
{
    [JsonPropertyName("idEmpresa")]
    public int IdEmpresa { get; set; }

    [JsonPropertyName("nombre")]
    public string Nombre { get; set; } = "";

    [JsonPropertyName("industria")]
    public string? Industria { get; set; }
}