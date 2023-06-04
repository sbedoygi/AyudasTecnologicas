using Microsoft.AspNetCore.Mvc;

namespace AyudasTecnologicas.Controllers
{
    public class CountriesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
