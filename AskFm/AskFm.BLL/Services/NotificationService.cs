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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHubContext<NotificationHub> _hubContext;


    public NotificationService(INotificationRepository notificationRepository, IUnitOfWork unitOfWork, IHubContext<NotificationHub> hubContext)
    {
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
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
                Message = notification.Message,
                IsRead = notification.IsRead,
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

    public async Task<List<NotificationDto>> GetNotificationsByType(int userId, string category, int pageNumber = 1, int pageSize = 10)
    {
        // Convert category to uppercase and match with enum
        if (!Enum.TryParse<NotificationStatus>(category.ToUpper(), out var notificationType))
            throw new ArgumentException($"Invalid notification category: {category}");

        var (notifications, totalCount) = await _notificationRepository.GetNotificationsByType(userId, notificationType, pageNumber, pageSize);

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
                Message = notification.Message,
                IsRead = notification.IsRead,
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
    public async Task<string> MarkNotificationAsRead(int notificationId, int userId)
    {
        var notification = await _notificationRepository.GetUserNotificationById(notificationId, userId);
        if (notification == null)
            throw new InvalidOperationException("Notification not found or access denied.");

        notification.IsRead = true;
        _unitOfWork.Notifications.Update(notification);
        await _unitOfWork.SaveAsync();

        return "notification has been read";
    }

    public async Task<string> MarkAllNotificationsAsRead(int userId)
    {
        var unreadNotifications = await _unitOfWork.Notifications.FindAllAsync(n => n.UserId == userId && !n.IsRead);

        foreach (var notification in unreadNotifications)
        {
            notification.IsRead = true;
            _unitOfWork.Notifications.Update(notification);
        }

        await _unitOfWork.SaveAsync();
        return "All notifications marked as read";
    }

    public async Task CreateNotification(int userId, NotificationStatus type, int resourceId, string message)
    {
        var notification = new Notification
        {
            UserId = userId,
            Type = type,
            ResourceId = resourceId,
            Message = message,
            IsRead = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Notifications.AddAsync(notification);
        await _unitOfWork.SaveAsync();

        // Get actor information for the notification
        var actorUser = await _notificationRepository.GetActorUserByResourceId(resourceId, type);

        var notificationDto = new NotificationDto
        {
            Id = notification.Id,
            UserId = notification.UserId,
            Type = notification.Type.ToString(),
            ResourceId = notification.ResourceId,
            Message = notification.Message,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt,
            Actor = actorUser == null ? null : new ActorDto
            {
                Id = actorUser.Id,
                Username = actorUser.UserName ?? "Unknown",
                AvatarPath = actorUser.AvatarPath ?? string.Empty
            }
        };

        // Send real-time notification to the specific user
        await _hubContext.Clients.Group($"user_{userId}")
            .SendAsync("ReceiveNotification", notificationDto);
    }
    
    
}