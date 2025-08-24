using AskFm.BLL.DTO;
using AskFm.BLL.Hub;
using AskFm.BLL.Services;
using AskFm.DAL.Enums;
using AskFm.DAL.Interfaces;
using AskFm.DAL.Models;
using Microsoft.AspNetCore.SignalR;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IHubContext<NotificationHub> _hubContext;


    public NotificationService(INotificationRepository notificationRepository, IHubContext<NotificationHub> hubContext)
    {
        _notificationRepository = notificationRepository;
        _hubContext = hubContext;
    }

    public async Task<List<NotificationDto>> GetUserNotifications(int userId, int pageNumber = 1, int pageSize = 10)
    {
        var (notifications, totalCount) = await _notificationRepository.GetAllNotifications(userId, pageNumber, pageSize);

        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        var notificationDtos = new List<NotificationDto>();

        foreach (var notification in notifications)
        {
            var actorUser = await _notificationRepository.GetActorUserByResourceId(notification.ResourceId, notification.Type);
            notificationDtos.Add(new NotificationDto
            {
                Id = notification.Id,
                UserId = notification.UserId,
                Type = notification.Type.ToString(),
                ResourceId = notification.ResourceId,
                Message = notification.jsonContent,
                IsRead = notification.isRead,
                CreatedAt = notification.CreatedAt,
                Actor = actorUser == null ? null : new ActorDto
                {
                    Id = actorUser.Id,
                    Username = actorUser?.UserName ?? "Unknown",
                    AvatarPath = actorUser?.AvatarPath ?? String.Empty
                },
                Pagination = new PaginationDto
                {
                    CurrentPage = pageNumber,
                    TotalPages = totalPages,
                    TotalCount = totalCount,
                    HasNext = pageNumber < totalPages,
                    HasPrevious = pageNumber > 1
                }
            });
        }
        return notificationDtos;
    }

    public async Task<NotificationTypeResponse> GetNotificationsByType(int userId, string category, int pageNumber = 1, int pageSize = 10)
    {
        // first we have to convert category to upper case and match it with enum
        if (!Enum.TryParse<NotificationStatus>(category, true, out var notificationType))
            throw new ArgumentException($"Invalid notification category: {category}");

        var (notifications, totalCount) = await _notificationRepository.GetNotificationsByType(userId, notificationType, pageNumber, pageSize);
        
        var notificationDtos = new List<NotificationDto>();

        foreach (var notification in notifications)
        {
            var actorUser = await _notificationRepository.GetActorUserByResourceId(notification.ResourceId, notification.Type);
            notificationDtos.Add(new NotificationDto
            { 
                Id = notification.Id,
                UserId = notification.UserId,
                Type = notification.Type.ToString(),
                ResourceId = notification.ResourceId,
                Message = notification.jsonContent,
                IsRead = notification.isRead,
                CreatedAt = notification.CreatedAt,
                Actor = actorUser == null ? null : new ActorDto
                {
                    Id = actorUser.Id,
                    Username = actorUser?.UserName ?? "Unknown",
                    AvatarPath = actorUser?.AvatarPath ?? String.Empty
                }
            });
        }

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

    public async Task CreateNotification(int userId, NotificationStatus type, int resourceId, string message)
    {
        var notification = new Notification
        {
            UserId = userId,
            Type = type,
            ResourceId = resourceId,
            jsonContent = message,
            isRead = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _notificationRepository.AddNotification(notification);
        
        var notificationDto = new NotificationDto
        {
            Id = notification.Id,
            UserId = notification.UserId,
            Type = notification.Type.ToString(),
            ResourceId = notification.ResourceId,
            Message = notification.jsonContent,
            IsRead = notification.isRead,
            CreatedAt = notification.CreatedAt
        };

        await _hubContext.Clients.Group($"user_{userId}")
            .SendAsync("ReceiveNotification", notificationDto);
    }
}