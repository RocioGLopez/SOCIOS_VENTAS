using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrototipoCompras.Data;
using PrototipoCompras.Models;
using PrototipoCompras.Services;

namespace PrototipoCompras.Controllers;

public class ProveedoresController : Controller
{
    private readonly AppDbContext _db;
    private readonly IBitacoraService _bitacora;

    public ProveedoresController(AppDbContext db, IBitacoraService bitacora)
    {
        _db = db;
        _bitacora = bitacora;
    }

    // GET: /Proveedores/CreateEval
    public IActionResult CreateEval()
    {
        ViewBag.Proveedores = _db.Proveedores.OrderBy(p => p.Nombre).ToList();
        return View(new EvaluacionProveedor());
    }

    // POST: /Proveedores/CreateEval
    [HttpPost]
    public async Task<IActionResult> CreateEval(EvaluacionProveedor model)
    {
        var start = DateTime.UtcNow;

        if (!ModelState.IsValid)
        {
            ViewBag.Proveedores = _db.Proveedores.OrderBy(p => p.Nombre).ToList();
            return View(model);
        }

        // Guardar fecha y usuario evaluador (demo)
        model.Fecha = DateTime.UtcNow;
        if (string.IsNullOrWhiteSpace(model.Evaluador))
            model.Evaluador = "ComprasDemo";

        _db.Evaluaciones.Add(model);
        await _db.SaveChangesAsync();

        await _bitacora.RegistrarAsync(
            "Proveedores",
            "Registrar evaluación",
            $"ProveedorId: {model.ProveedorId}, score: {model.Score}, evaluador: {model.Evaluador}",
            model.Id);

        TempData["TiempoMs"] = (int)Math.Round((DateTime.UtcNow - start).TotalMilliseconds);
        return RedirectToAction(nameof(History));
    }

    // GET: /Proveedores/History
    public IActionResult History()
    {
        try
        {
            var list = _db.Evaluaciones
                .Include(e => e.Proveedor)
                .OrderByDescending(e => e.Id)
                .ToList();

            ViewBag.TiempoMs = TempData["TiempoMs"];
            return View(list);
        }
        catch (Exception)
        {
            ViewBag.Error = "No se pudo conectar a la base de datos. Intenta más tarde.";
            return View(new List<EvaluacionProveedor>());
        }
    }
}