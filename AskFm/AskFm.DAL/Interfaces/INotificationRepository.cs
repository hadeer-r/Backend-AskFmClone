using AskFm.DAL.Enums;
using AskFm.DAL.Models;

namespace AskFm.DAL.Interfaces;

public interface INotificationRepository
{
    Task<IEnumerable<Notification>> GetAllNotifications(int userId, int pageNumber, int pageSize);
    Task<Notification> GetNotificationById(int notificationId);
    Task UpdateNotification(Notification notification);
    Task AddNotification(Notification notification);
    Task<IQueryable<Notification>> GetNotificationsByType(int userId, NotificationStatus status, int pageNumber, int pageSize);
}