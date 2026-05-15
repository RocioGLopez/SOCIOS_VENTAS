using PrototipoCompras.Data;
using PrototipoCompras.Models;

namespace PrototipoCompras.Services;

public class BitacoraService : IBitacoraService
{
    private readonly AppDbContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BitacoraService(AppDbContext db, IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task RegistrarAsync(
        string modulo,
        string accion,
        string? detalle = null,
        int? entidadId = null,
        bool exitoso = true,
        string? usuarioOverride = null,
        CancellationToken cancellationToken = default)
    {
        var http = _httpContextAccessor.HttpContext;
        var usuario = usuarioOverride
            ?? http?.User?.Identity?.Name
            ?? "Sistema";

        if (detalle?.Length > 1000)
            detalle = detalle[..1000];

        _db.Bitacoras.Add(new Bitacora
        {
            Fecha = DateTime.UtcNow,
            Usuario = usuario,
            Modulo = modulo,
            Accion = accion,
            Detalle = detalle,
            EntidadId = entidadId,
            IpAddress = http?.Connection?.RemoteIpAddress?.ToString(),
            Exitoso = exitoso
        });

        await _db.SaveChangesAsync(cancellationToken);
    }
}
