using Microsoft.AspNetCore.Mvc;

namespace AyudasTecnologicas.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
