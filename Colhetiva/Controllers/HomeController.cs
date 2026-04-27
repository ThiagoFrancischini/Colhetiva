using Microsoft.AspNetCore.Mvc;

namespace Colhetiva.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}