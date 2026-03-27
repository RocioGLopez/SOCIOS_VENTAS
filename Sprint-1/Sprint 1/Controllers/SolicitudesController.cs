using Microsoft.AspNetCore.Mvc;
using PrototipoCompras.Data;
using PrototipoCompras.Models;

namespace PrototipoCompras.Controllers;

public class SolicitudesController : Controller
{
    private readonly AppDbContext _db;
    public SolicitudesController(AppDbContext db) => _db = db;

    // GET: /Solicitudes/Create
    public IActionResult Create()
    {
        return View(new SolicitudCompra { Fecha = DateTime.Today });
    }

    // POST: /Solicitudes/Create
    [HttpPost]
    public async Task<IActionResult> Create(SolicitudCompra model)
    {
        var start = DateTime.UtcNow;

        if (!ModelState.IsValid)
            return View(model);

        // Criterio: estado automático
        model.Estado = "Pendiente de aprobación";

        if (string.IsNullOrWhiteSpace(model.Solicitante))
            model.Solicitante = "EmpleadoDemo";

        _db.Solicitudes.Add(model);
        await _db.SaveChangesAsync();

        ViewBag.TiempoMs = (int)Math.Round((DateTime.UtcNow - start).TotalMilliseconds);
        return View("Success", model);
    }

    // Panel del jefe
    public IActionResult BossPanel()
    {
        var list = _db.Solicitudes.OrderByDescending(s => s.Id).ToList();
        return View(list);
    }

    // Acciones demo
    public async Task<IActionResult> Approve(int id)
    {
        var s = await _db.Solicitudes.FindAsync(id);
        if (s != null) { s.Estado = "Aprobada"; await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(BossPanel));
    }

    public async Task<IActionResult> Reject(int id)
    {
        var s = await _db.Solicitudes.FindAsync(id);
        if (s != null) { s.Estado = "Rechazada"; await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(BossPanel));
    }
}