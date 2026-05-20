namespace PrototipoCompras.Models;

public class ContactosApiResultadoViewModel
{
    public List<ContactoExternoDto> Contactos { get; set; } = new();
    public string? Error { get; set; }
    public string? RawResponse { get; set; }
}