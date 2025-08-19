using AskFm.BLL.DTO;
using AskFm.BLL.Services;
using AskFm.DAL.Enums;
using AskFm.DAL.Interfaces;
using AskFm.DAL.Models;
using AutoMapper;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IMapper _mapper;

    public NotificationService(INotificationRepository notificationRepository, IMapper mapper)
    {
        _notificationRepository = notificationRepository;
        _mapper = mapper;
    }

    public async Task<List<NotificationDto>> GetUserNotifications(int userId, int pageNumber = 1, int pageSize = 10)
    {
        var (notifications, totalCount) = await _notificationRepository.GetAllNotifications(userId, pageNumber, pageSize);
        return _mapper.Map<List<NotificationDto>>(notifications);
    }

    public async Task<NotificationCategoryResponse> GetNotificationsByCategory(int userId, string category, int pageNumber = 1, int pageSize = 10)
    {
        if (!Enum.TryParse<NotificationStatus>(category, true, out var notificationType))
            throw new ArgumentException($"Invalid notification category: {category}");

        var (notifications, totalCount) = await _notificationRepository.GetNotificationsByType(userId, notificationType, pageNumber, pageSize);
        
        var notificationDtos = _mapper.Map<List<NotificationDto>>(notifications);
        
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        
        return new NotificationCategoryResponse
        {
            Category = category.ToUpper(),
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
        return "noti has been read";
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