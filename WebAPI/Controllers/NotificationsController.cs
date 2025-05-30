using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using WebAPI.DTOs;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetAll()
        {
            try
            {
                var notifications = await _notificationService.GetAllNotificationsAsync();
                Log.Information("All notifications fetched successfully!");
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NotificationDto>> GetById(int id)
        {
            try
            {
                var notification = await _notificationService.GetNotificationByIdAsync(id);

                if (notification == null)
                {
                    Log.Warning($"Notification with ID {id} not found");
                    return NotFound();
                }

                return Ok(notification);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetByUser(int userId)
        {
            try
            {
                var notifications = await _notificationService.GetNotificationsByUserIdAsync(userId);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<NotificationDto>> Create(NotificationDto notificationDto)
        {
            try
            {
                var createdNotification = await _notificationService.CreateNotificationAsync(notificationDto);
                return CreatedAtAction(nameof(GetById), new { id = createdNotification.Id }, createdNotification);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, NotificationDto notificationDto)
        {
            if (id != notificationDto.Id)
                return BadRequest("ID mismatch");

            try
            {
                await _notificationService.UpdateNotificationAsync(id, notificationDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                Log.Error(ex.Message);
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _notificationService.DeleteNotificationAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                Log.Error(ex.Message);
                return NotFound();
            }
        }

        [HttpGet("paged")]
        public async Task<ActionResult> GetPaged([FromQuery] NotificationQueryDto query)
        {
            try
            {
                var (notifications, totalCount) = await _notificationService.GetNotificationsPagedAsync(query);

                Response.Headers.Add("X-Total-Count", totalCount.ToString());
                Response.Headers.Add("X-Page-Size", query.PageSize.ToString());
                Response.Headers.Add("X-Current-Page", query.Page.ToString());

                return Ok(notifications);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        [HttpGet("options")]
        public ActionResult GetOptions()
        {
            try
            {
                return Ok(new
                {
                    PageSizes = _notificationService.GetAvailablePageSizes(),
                    SortColumns = _notificationService.GetAvailableSortColumns()
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return NotFound(ex.Message);
            }
        }
    }
}
