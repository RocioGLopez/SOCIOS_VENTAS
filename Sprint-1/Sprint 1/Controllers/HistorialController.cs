using Microsoft.AspNetCore.Mvc;

namespace PrototipoCompras.Controllers
{
    public class HistorialController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}