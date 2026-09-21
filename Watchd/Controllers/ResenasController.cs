using Microsoft.AspNetCore.Mvc;
using Watchd.Models;

namespace Watchd.Controllers
{
    public class ResenasController : Controller
    {
        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Crear(Visualizacion visualizacion)
        {
            if (!ModelState.IsValid)
            {
                return View(visualizacion);
            }

            return View();
        }
    }
}