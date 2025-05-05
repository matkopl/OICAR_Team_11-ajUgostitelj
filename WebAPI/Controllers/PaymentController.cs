using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet("get_all")]
        public async Task<IActionResult> GetAllPayments()
        {
            var payments = await _paymentService.GetAllPaymentsAsync();
            return Ok(payments);
        }

        [HttpGet("get/{id}")] 
        public async Task<IActionResult> GetPaymentById(int id)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);
            return Ok(payment);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment(PaymentDto paymentDto)
        {
            var newPayment = await _paymentService.CreatePaymentAsync(paymentDto);
            return Ok(newPayment);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdatePayment(int id, PaymentDto paymentDto)
        {
            paymentDto.Id = id;
            var success = await _paymentService.UpdatePaymentAsync(paymentDto);
            return success ? Ok("Payment updated successfully") : NotFound("Payment not found");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var success = await _paymentService.DeletePaymentAsync(id);
            return success ? Ok("Payment deleted successfully") : NotFound("Payment not found");
        }
    }
}
