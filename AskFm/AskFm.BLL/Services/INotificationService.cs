using AskFm.BLL.DTO;
using AskFm.DAL.Enums;

namespace AskFm.BLL.Services;

public interface INotificationService
{
    Task<List<NotificationDto>> GetUserNotifications(int userId, int pageNumber = 1, int pageSize = 10);
    Task<NotificationTypeResponse> GetNotificationsByType(int userId, string category, int pageNumber = 1, int pageSize = 10);
    Task<string> MarkNotificationAsRead(int notificationId);
    Task<string> MarkAllNotificationsAsRead(int userId);
    Task CreateNotification(int userId, NotificationStatus type, int resourceId, string message);
}