using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using WebAPI.DTOs;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();
                Log.Information("All orders fetched successfully!");
                return Ok(orders);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetById(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                {
                    Log.Warning($"Order with ID {id} not found");
                    return NotFound();
                }
                return Ok(order);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, $"Error fetching order with ID {id}");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> Create(OrderDto orderDto)
        {
            try
            {
                Log.Information($"Creating order...");
                var createdOrder = await _orderService.CreateOrderAsync(orderDto);
                Log.Information($"Successfully created with ID:{createdOrder.Id}");
                return CreatedAtAction(nameof(GetById), new { id = createdOrder.Id }, createdOrder);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating order");
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, OrderDto orderDto)
        {
            if (id != orderDto.Id)
            {
                Log.Error($"ID in URL does not match ID in body)", id, orderDto.Id);
                return BadRequest($"ID in URL does not match ID in body");
            }

            try
            {
                Log.Information($"Updating order with ID {id}...");
                await _orderService.UpdateOrderAsync(id, orderDto);
                Log.Information($"Order with ID {id} updated successfully");
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                Log.Error(ex, $"Order with ID {id} not found during update", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error updating order with ID {id}");
                return StatusCode(500, "Error occurred");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                Log.Information($"Deleting order ID:{id}...");
                await _orderService.DeleteOrderAsync(id);
                Log.Information($"Successfully deleted table with ID {id}");
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                Log.Error(ex, $"Order with ID {id} not found during deletion");
                return NotFound();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error while deleting order with ID {id}");
                return StatusCode(500, "Error with request!");
            }
        }

        [HttpGet("{id}/status")]
        public async Task<ActionResult<string>> GetStatus(int id)
        {
            try
            {
                Log.Information($"Getting status of order ID:{id}...");
                var status = await _orderService.GetOrderStatusAsync(id);
                Log.Information($"Status of order {status}!");
                return Ok(new { Status = status });
            }
            catch (KeyNotFoundException ex)
            {
                Log.Error(ex, $"Status with ID {id} not found!");
                return NotFound();
            }
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatusDto statusDto)
        {
            if (id != statusDto.OrderId)
            {
                Log.Error($"Order ID mismatch");
                return BadRequest("Order ID mismatch");
            }

            try
            {
                Log.Information($"Updating order status with ID:{id}...");
                await _orderService.UpdateOrderStatusAsync(statusDto);
                Log.Information($"Order updated!");
                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error updating order status  with ID {id}");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("status-definitions")]
        public async Task<ActionResult> GetStatusDefinitions()
        {
            try
            {
                Log.Information($"Getting status definitions...");
                var definitions = await _orderService.GetStatusDefinitionsAsync();
                Log.Information($"Status definitions {definitions.Values}");
                return Ok(definitions);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error getting status definitions!");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("paged")]
        public async Task<ActionResult> GetPaged([FromQuery] OrderQueryDto query)
        {
            var (orders, totalCount) = await _orderService.GetOrdersPagedAsync(query);

            Response.Headers.Add("X-Total-Count", totalCount.ToString());
            Response.Headers.Add("X-Page-Size", query.PageSize.ToString());
            Response.Headers.Add("X-Current-Page", query.Page.ToString());

            return Ok(orders);
        }

        [HttpGet("options")]
        public ActionResult GetOptions()
        {
            return Ok(new
            {
                PageSizes = _orderService.GetAvailablePageSizes(),
                SortColumns = _orderService.GetAvailableSortColumns()
            });
        }
    }
}
