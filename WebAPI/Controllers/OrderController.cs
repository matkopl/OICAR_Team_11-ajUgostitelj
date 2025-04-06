﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            return order != null ? Ok(order) : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> Create(OrderDto orderDto)
        {
            var createdOrder = await _orderService.CreateOrderAsync(orderDto);
            return CreatedAtAction(nameof(GetById), new { id = createdOrder.Id }, createdOrder);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, OrderDto orderDto)
        {
            try
            {
                await _orderService.UpdateOrderAsync(id, orderDto);
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
                await _orderService.DeleteOrderAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("{id}/status")]
        public async Task<ActionResult<string>> GetStatus(int id)
        {
            try
            {
                var status = await _orderService.GetOrderStatusAsync(id);
                return Ok(new { Status = status });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPut("{id}/status")] 
        public async Task<IActionResult> UpdateStatus(int id, OrderStatusDto statusDto)
        {
            if (id != statusDto.OrderId)
                return BadRequest("Order ID mismatch");

            try
            {
                await _orderService.UpdateOrderStatusAsync(statusDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet("status-definitions")]
        public async Task<ActionResult> GetStatusDefinitions()
        {
            var definitions = await _orderService.GetStatusDefinitionsAsync();
            return Ok(definitions);
        }

        // OrderController.cs
        [HttpGet("paged")]
        public async Task<ActionResult> GetPaged([FromQuery] OrderQueryDto query)
        {
            try
            {
                var result = await _orderService.GetOrdersPagedAsync(query);

                Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
                Response.Headers.Add("X-Page-Size", query.PageSize.ToString());
                Response.Headers.Add("X-Current-Page", query.Page.ToString());

                return Ok(result.Orders ?? new List<OrderDto>());
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
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
