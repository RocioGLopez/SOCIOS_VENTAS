using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrototipoCompras.Data;
using PrototipoCompras.Models;

namespace PrototipoCompras.Controllers;

[Authorize]
public class BitacoraController : Controller
{
    private readonly AppDbContext _db;

    public BitacoraController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(
        string? modulo,
        string? usuario,
        DateTime? desde,
        DateTime? hasta,
        bool? soloErrores)
    {
        var query = _db.Bitacoras.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(modulo))
            query = query.Where(b => b.Modulo == modulo);

        if (!string.IsNullOrWhiteSpace(usuario))
            query = query.Where(b => b.Usuario.Contains(usuario));

        if (desde.HasValue)
            query = query.Where(b => b.Fecha >= desde.Value.ToUniversalTime());

        if (hasta.HasValue)
        {
            var fin = hasta.Value.Date.AddDays(1).ToUniversalTime();
            query = query.Where(b => b.Fecha < fin);
        }

        if (soloErrores == true)
            query = query.Where(b => !b.Exitoso);

        var registros = await query
            .OrderByDescending(b => b.Fecha)
            .Take(500)
            .ToListAsync();

        ViewBag.Modulo = modulo;
        ViewBag.Usuario = usuario;
        ViewBag.Desde = desde?.ToString("yyyy-MM-dd");
        ViewBag.Hasta = hasta?.ToString("yyyy-MM-dd");
        ViewBag.SoloErrores = soloErrores;
        ViewBag.Modulos = await _db.Bitacoras
            .AsNoTracking()
            .Select(b => b.Modulo)
            .Distinct()
            .OrderBy(m => m)
            .ToListAsync();

        return View(registros);
    }
}
