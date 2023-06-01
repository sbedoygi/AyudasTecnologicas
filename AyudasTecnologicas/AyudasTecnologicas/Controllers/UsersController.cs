using Microsoft.AspNetCore.Mvc;

namespace AyudasTecnologicas.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
