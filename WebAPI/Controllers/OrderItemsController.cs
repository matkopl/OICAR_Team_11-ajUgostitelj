using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemsController : ControllerBase
    {
        private readonly IOrderItemService _orderItemService;

        public OrderItemsController(IOrderItemService orderItemService)
        {
            _orderItemService = orderItemService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderItemDto>>> GetAll()
        {
            var orderItems = await _orderItemService.GetAllOrderItemsAsync();
            return Ok(orderItems);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderItemDto>> GetById(int id)
        {
            var orderItem = await _orderItemService.GetOrderItemByIdAsync(id);
            return orderItem != null ? Ok(orderItem) : NotFound();
        }

        [HttpGet("order/{orderId}")]
        public async Task<ActionResult<IEnumerable<OrderItemDto>>> GetByOrderId(int orderId)
        {
            var orderItems = await _orderItemService.GetOrderItemsByOrderIdAsync(orderId);
            return Ok(orderItems);
        }

        [HttpPost]
        public async Task<ActionResult<OrderItemDto>> Create(OrderItemDto orderItemDto)
        {
            try
            {
                var createdOrderItem = await _orderItemService.CreateOrderItemAsync(orderItemDto);
                return CreatedAtAction(nameof(GetById), new { id = createdOrderItem.Id }, createdOrderItem);
            }
            catch (Exception ex) when (ex is KeyNotFoundException || ex is InvalidOperationException)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, OrderItemDto orderItemDto)
        {
            try
            {
                await _orderItemService.UpdateOrderItemAsync(id, orderItemDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _orderItemService.DeleteOrderItemAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}