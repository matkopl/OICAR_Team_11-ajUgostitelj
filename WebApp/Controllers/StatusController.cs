using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebAPI.DTOs;
using WebApp.ApiClients;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class StatusController : Controller
    {
        private readonly OrderApiClient _orderApiClient;

        public StatusController(OrderApiClient orderApiClient)
        {
            _orderApiClient = orderApiClient;
        }

        public async Task<IActionResult> Index()
        {
            var lastOrderId = HttpContext.Session.GetInt32("LastOrderId");
            if (lastOrderId == null)
                return RedirectToAction("Index", "Product");

            var order = await _orderApiClient.GetOrderById(lastOrderId.Value);
            if (order == null)
                return NotFound("Order not found.");

            return View(order); 
        }
    }
}
