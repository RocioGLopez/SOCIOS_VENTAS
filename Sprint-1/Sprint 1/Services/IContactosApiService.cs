using PrototipoCompras.Models;

namespace PrototipoCompras.Services;

public interface IContactosApiService
{
    Task<List<ContactoExternoDto>> ObtenerContactosAsync();
}