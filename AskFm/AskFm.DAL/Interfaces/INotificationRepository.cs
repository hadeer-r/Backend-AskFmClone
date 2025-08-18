using AskFm.DAL.Enums;
using AskFm.DAL.Models;

namespace AskFm.DAL.Interfaces;

public interface INotificationRepository
{
    Task<(IEnumerable<Notification> notifications, int totalCount)> GetAllNotifications(int userId, int pageNumber, int pageSize);
    Task<Notification> GetNotificationById(int notificationId);
    Task UpdateNotification(Notification notification);
    Task AddNotification(Notification notification);
    Task<(IEnumerable<Notification> notifications, int totalCount)> GetNotificationsByType(int userId, NotificationStatus status, int pageNumber, int pageSize);
    Task MarkNotificationAsRead(int notificationId);
    Task MarkAllNotificationsAsRead(int userId);
}