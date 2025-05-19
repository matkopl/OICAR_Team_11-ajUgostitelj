using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebAPI.DTOs;
using WebApp.ApiClients;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class StatusController : Controller
    {

        public async Task<IActionResult> Index([FromServices] OrderApiClient orderApi)
        {
            var lastOrderId = HttpContext.Session.GetInt32("LastOrderId");
            if (lastOrderId == null)
                return RedirectToAction("Index", "Product");

            var order = await orderApi.GetOrderByIdAsync(lastOrderId.Value);
            if (order == null)
                return NotFound("Order not found.");

            return View(order); 
        }
    }
}
