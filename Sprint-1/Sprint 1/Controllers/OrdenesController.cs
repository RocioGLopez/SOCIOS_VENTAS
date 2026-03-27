using Microsoft.AspNetCore.Mvc;

namespace PrototipoCompras.Controllers
{
    public class OrdenesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}