using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
