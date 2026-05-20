using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrototipoCompras.Services;

namespace PrototipoCompras.Controllers;

[Authorize]
public class IntegracionContactosController : Controller
{
    private readonly IContactosApiService _service;

    public IntegracionContactosController(IContactosApiService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index()
    {
        var contactos = await _service.ObtenerContactosAsync();
        return View(contactos);
    }
}