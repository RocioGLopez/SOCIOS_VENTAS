using System.Text.Json.Serialization;

namespace PrototipoCompras.Models;

public class ContactoExternoDto
{
    // Posibles nombres para ID
    public int Id { get; set; }

    [JsonPropertyName("idContacto")]
    public int? IdContacto { get; set; }

    [JsonPropertyName("contactoId")]
    public int? ContactoId { get; set; }

    // Nombre y apellido
    public string Nombre { get; set; } = "";

    public string? Apellido { get; set; }

    public string? Apellidos { get; set; }

    [JsonPropertyName("nombreCompleto")]
    public string? NombreCompletoApi { get; set; }

    // Teléfono
    public string Telefono { get; set; } = "";

    // Posibles nombres para correo
    public string? Correo { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("correoElectronico")]
    public string? CorreoElectronico { get; set; }

    // Posibles nombres para empresa
    public string? Empresa { get; set; }

    [JsonPropertyName("nombreEmpresa")]
    public string? NombreEmpresa { get; set; }

    [JsonPropertyName("empresaNombre")]
    public string? EmpresaNombre { get; set; }

    [JsonPropertyName("company")]
    public string? Company { get; set; }

    [JsonIgnore]
    public int IdFinal =>
        Id != 0 ? Id :
        IdContacto ?? ContactoId ?? 0;

    [JsonIgnore]
    public string NombreCompletoFinal
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(NombreCompletoApi))
                return NombreCompletoApi!;

            var apellidoFinal = !string.IsNullOrWhiteSpace(Apellido)
                ? Apellido
                : Apellidos;

            return string.IsNullOrWhiteSpace(apellidoFinal)
                ? Nombre
                : $"{Nombre} {apellidoFinal}".Trim();
        }
    }

    [JsonIgnore]
    public string CorreoFinal =>
        !string.IsNullOrWhiteSpace(Correo) ? Correo! :
        !string.IsNullOrWhiteSpace(Email) ? Email! :
        !string.IsNullOrWhiteSpace(CorreoElectronico) ? CorreoElectronico! :
        "";

    [JsonIgnore]
    public string EmpresaFinal =>
        !string.IsNullOrWhiteSpace(Empresa) ? Empresa! :
        !string.IsNullOrWhiteSpace(NombreEmpresa) ? NombreEmpresa! :
        !string.IsNullOrWhiteSpace(EmpresaNombre) ? EmpresaNombre! :
        !string.IsNullOrWhiteSpace(Company) ? Company! :
        "";
}