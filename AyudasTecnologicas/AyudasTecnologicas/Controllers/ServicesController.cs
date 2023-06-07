using Microsoft.AspNetCore.Mvc;

namespace AyudasTecnologicas.Controllers
{
    public class ServicesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
