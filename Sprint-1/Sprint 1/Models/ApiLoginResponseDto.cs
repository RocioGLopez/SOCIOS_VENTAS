using System.Text.Json.Serialization;

namespace PrototipoCompras.Models;

public class ApiLoginResponseDto
{
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    [JsonPropertyName("accessToken")]
    public string? AccessToken { get; set; }

    [JsonPropertyName("jwt")]
    public string? Jwt { get; set; }
}