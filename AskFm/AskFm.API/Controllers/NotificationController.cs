using AskFm.BLL.Services;
using AskFm.DAL.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AskFm.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserNotifications(int userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var notifications = await _notificationService.GetUserNotifications(userId, pageNumber, pageSize);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{userId}/type/{category}")]
        public async Task<IActionResult> GetNotificationsByType(int userId, string category, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _notificationService.GetNotificationsByType(userId, category, pageNumber, pageSize);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{notificationId}/read")]
        public async Task<IActionResult> MarkNotificationAsRead(int notificationId)
        {
            try
            {
                var result = await _notificationService.MarkNotificationAsRead(notificationId);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{userId}/read-all")]
        public async Task<IActionResult> MarkAllNotificationsAsRead(int userId)
        {
            try
            {
                var result = await _notificationService.MarkAllNotificationsAsRead(userId);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateNotification(
            int userId,
            int? actorUserId,
            NotificationStatus type,
            int resourceId,
            string message)
        {
            try
            {
                await _notificationService.CreateNotification(
                    userId, 
                    actorUserId, 
                    type, 
                    resourceId, 
                    message);

                return CreatedAtAction(nameof(GetUserNotifications), new { userId = userId }, new { message = "Notification created successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}