using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using PrototipoCompras.Models;

namespace PrototipoCompras.Services;

public class ContactosApiService : IContactosApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ContactosApiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<ContactosApiResultadoViewModel> ObtenerContactosAsync()
    {
        var resultado = new ContactosApiResultadoViewModel();

        var token = await ObtenerTokenAsync(
            _configuration["ExternalApis:VendedorEmail"] ?? "",
            _configuration["ExternalApis:VendedorPassword"] ?? "");

        if (string.IsNullOrWhiteSpace(token))
        {
            resultado.Error = "No se pudo obtener token. Revisa LoginPath, correo o contraseña.";
            return resultado;
        }

        var endpoint = _configuration["ExternalApis:ContactosPath"] ?? "/api/Contactos";

        using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        var raw = await response.Content.ReadAsStringAsync();

        resultado.RawResponse = raw;

        if (!response.IsSuccessStatusCode)
        {
            resultado.Error = $"Error {(int)response.StatusCode}: {response.ReasonPhrase}";
            return resultado;
        }

        try
        {
            var opciones = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var listaDirecta = JsonSerializer.Deserialize<List<ContactoExternoDto>>(raw, opciones);
            if (listaDirecta != null)
            {
                resultado.Contactos = listaDirecta;
                return resultado;
            }
        }
        catch
        {
        }

        try
        {
            using var doc = JsonDocument.Parse(raw);

            if (doc.RootElement.ValueKind == JsonValueKind.Object)
            {
                // Busca si la API devuelve algo tipo { data: [...] } o { contactos: [...] }
                if (doc.RootElement.TryGetProperty("data", out var dataProp) &&
                    dataProp.ValueKind == JsonValueKind.Array)
                {
                    var lista = JsonSerializer.Deserialize<List<ContactoExternoDto>>(dataProp.GetRawText(),
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    resultado.Contactos = lista ?? new List<ContactoExternoDto>();
                    return resultado;
                }

                if (doc.RootElement.TryGetProperty("contactos", out var contactosProp) &&
                    contactosProp.ValueKind == JsonValueKind.Array)
                {
                    var lista = JsonSerializer.Deserialize<List<ContactoExternoDto>>(contactosProp.GetRawText(),
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    resultado.Contactos = lista ?? new List<ContactoExternoDto>();
                    return resultado;
                }
            }
        }
        catch
        {
        }

        resultado.Error = "La API respondió, pero con una estructura distinta a la esperada.";
        return resultado;
    }

    private async Task<string?> ObtenerTokenAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return null;

        var loginPath = _configuration["ExternalApis:LoginPath"] ?? "/api/Auth/login";

        var request = new ApiLoginRequestDto
        {
            Email = email,
            Password = password
        };

        var response = await _httpClient.PostAsJsonAsync(loginPath, request);

        if (!response.IsSuccessStatusCode)
            return null;

        var raw = await response.Content.ReadAsStringAsync();

        try
        {
            var parsed = JsonSerializer.Deserialize<ApiLoginResponseDto>(raw, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return parsed?.Token
                ?? parsed?.AccessToken
                ?? parsed?.Jwt;
        }
        catch
        {
            try
            {
                using var doc = JsonDocument.Parse(raw);

                if (doc.RootElement.TryGetProperty("token", out var token))
                    return token.GetString();

                if (doc.RootElement.TryGetProperty("accessToken", out var accessToken))
                    return accessToken.GetString();

                if (doc.RootElement.TryGetProperty("jwt", out var jwt))
                    return jwt.GetString();
            }
            catch
            {
            }

            return null;
        }
    }
}