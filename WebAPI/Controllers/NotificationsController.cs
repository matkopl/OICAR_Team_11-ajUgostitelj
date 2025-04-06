using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs;
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
            var notifications = await _notificationService.GetAllNotificationsAsync();
            return Ok(notifications);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NotificationDto>> GetById(int id)
        {
            var notification = await _notificationService.GetNotificationByIdAsync(id);
            return notification != null ? Ok(notification) : NotFound();
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetByUser(int userId)
        {
            var notifications = await _notificationService.GetNotificationsByUserIdAsync(userId);
            return Ok(notifications);
        }

        [HttpPost]
        public async Task<ActionResult<NotificationDto>> Create(NotificationDto notificationDto)
        {
            var createdNotification = await _notificationService.CreateNotificationAsync(notificationDto);
            return CreatedAtAction(nameof(GetById), new { id = createdNotification.Id }, createdNotification);
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
                await _notificationService.DeleteNotificationAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("paged")]
        public async Task<ActionResult> GetPaged([FromQuery] NotificationQueryDto query)
        {
            var (notifications, totalCount) = await _notificationService.GetNotificationsPagedAsync(query);

            Response.Headers.Add("X-Total-Count", totalCount.ToString());
            Response.Headers.Add("X-Page-Size", query.PageSize.ToString());
            Response.Headers.Add("X-Current-Page", query.Page.ToString());

            return Ok(notifications);
        }

        [HttpGet("options")]
        public ActionResult GetOptions()
        {
            return Ok(new
            {
                PageSizes = _notificationService.GetAvailablePageSizes(),
                SortColumns = _notificationService.GetAvailableSortColumns()
            });
        }
    }
}
