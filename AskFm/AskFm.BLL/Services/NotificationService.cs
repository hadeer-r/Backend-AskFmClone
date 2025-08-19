using AskFm.BLL.DTO;
using AskFm.BLL.Services;
using AskFm.DAL.Enums;
using AskFm.DAL.Interfaces;
using AskFm.DAL.Models;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<List<NotificationDto>> GetUserNotifications(int userId, int pageNumber = 1, int pageSize = 10)
    {
        var (notifications, totalCount) = await _notificationRepository.GetAllNotifications(userId, pageNumber, pageSize);
        
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        var notificationDtos = notifications.Select(notification => new NotificationDto
        {
            Id = notification.Id,
            UserId = notification.UserId,
            Type = notification.Type.ToString(),
            ResourceId = notification.ResourceId,
            Message = notification.jsonContent,
            IsRead = notification.isRead,
            CreatedAt = notification.CreatedAt,
            Actor = new ActorDto
            {
                Id = notification.ActorUser?.Id ?? 0,
                Username = notification.ActorUser?.UserName ?? "Unknown",
                AvatarPath = notification.ActorUser?.AvatarPath ?? string.Empty
            },
            Pagination = new PaginationDto
            {
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                TotalCount = totalCount,
                HasNext = pageNumber < totalPages,
                HasPrevious = pageNumber > 1
            }
        }).ToList();

        return notificationDtos;
    }

    public async Task<NotificationTypeResponse> GetNotificationsByType(int userId, string category, int pageNumber = 1, int pageSize = 10)
    {
        if (!Enum.TryParse<NotificationStatus>(category, true, out var notificationType))
            throw new ArgumentException($"Invalid notification category: {category}");

        var (notifications, totalCount) = await _notificationRepository.GetNotificationsByType(userId, notificationType, pageNumber, pageSize);
        
        var notificationDtos = notifications.Select(notification => new NotificationDto
        {
            Id = notification.Id,
            UserId = notification.UserId,
            Type = notification.Type.ToString(),
            ResourceId = notification.ResourceId,
            Message = notification.jsonContent,
            IsRead = notification.isRead,
            CreatedAt = notification.CreatedAt,
            Actor = new ActorDto
            {
                Id = notification.ActorUser?.Id ?? 0,
                Username = notification.ActorUser?.UserName ?? "Unknown",
                AvatarPath = notification.ActorUser?.AvatarPath ?? string.Empty
            }
        }).ToList();
        
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        
        return new NotificationTypeResponse
        {
            Type = category.ToUpper(),
            Notifications = notificationDtos,
            Pagination = new PaginationDto
            {
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                TotalCount = totalCount,
                HasNext = pageNumber < totalPages,
                HasPrevious = pageNumber > 1
            }
        };
    }

    public async Task<string> MarkNotificationAsRead(int notificationId)
    {
        await _notificationRepository.MarkNotificationAsRead(notificationId);
        return "notification has been read";
    }

    public async Task<string> MarkAllNotificationsAsRead(int userId)
    {
        await _notificationRepository.MarkAllNotificationsAsRead(userId);
        return "All notifications marked as read";
    }

    public async Task CreateNotification(int userId, int? actorUserId, NotificationStatus type, int resourceId, string message)
    {
        var notification = new Notification
        {
            UserId = userId,
            ActorUserId = actorUserId,
            Type = type,
            ResourceId = resourceId,
            jsonContent = message,
            isRead = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _notificationRepository.AddNotification(notification);
    }
}