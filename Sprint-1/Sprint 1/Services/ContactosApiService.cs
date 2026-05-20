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

    public async Task<List<ContactoExternoDto>> ObtenerContactosAsync()
    {
        var token = await ObtenerTokenAsync(
            _configuration["ExternalApis:VendedorEmail"] ?? "",
            _configuration["ExternalApis:VendedorPassword"] ?? "");

        if (string.IsNullOrWhiteSpace(token))
            return new List<ContactoExternoDto>();

        var endpoint = _configuration["ExternalApis:ContactosPath"] ?? "/api/Contactos";

        using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            return new List<ContactoExternoDto>();

        var raw = await response.Content.ReadAsStringAsync();

        try
        {
            var data = JsonSerializer.Deserialize<List<ContactoExternoDto>>(raw, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return data ?? new List<ContactoExternoDto>();
        }
        catch
        {
            return new List<ContactoExternoDto>();
        }
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