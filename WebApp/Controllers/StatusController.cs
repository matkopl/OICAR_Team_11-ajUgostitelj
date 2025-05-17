using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class StatusController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
