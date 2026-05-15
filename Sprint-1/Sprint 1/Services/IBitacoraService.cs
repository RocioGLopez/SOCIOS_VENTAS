namespace PrototipoCompras.Services;

public interface IBitacoraService
{
    Task RegistrarAsync(
        string modulo,
        string accion,
        string? detalle = null,
        int? entidadId = null,
        bool exitoso = true,
        string? usuarioOverride = null,
        CancellationToken cancellationToken = default);
}
