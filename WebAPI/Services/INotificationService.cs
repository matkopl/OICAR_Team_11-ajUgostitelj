using System.Linq.Expressions;
using WebAPI.DTOs;

namespace WebAPI.Services
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationDto>> GetAllNotificationsAsync();
        Task<NotificationDto?> GetNotificationByIdAsync(int id);
        Task<IEnumerable<NotificationDto>> GetNotificationsByUserIdAsync(int userId);
        Task<NotificationDto> CreateNotificationAsync(NotificationDto notificationDto);
        Task UpdateNotificationAsync(int id, NotificationDto notificationDto);
        Task DeleteNotificationAsync(int id);
    }
}
