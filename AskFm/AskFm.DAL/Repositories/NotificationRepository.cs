using AskFm.DAL.Enums;
using AskFm.DAL.Interfaces;
using AskFm.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace AskFm.DAL.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;
    
    public NotificationRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<(IEnumerable<Notification> notifications, int totalCount)> GetAllNotifications(int userId, int pageNumber, int pageSize)
    {
        var query = _context.Notifications
            .Include(n => n.ActorUser)
            .Where(n => n.UserId == userId);
            
        var totalCount = await query.CountAsync();
        
        var notifications = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
            
        return (notifications, totalCount);
    }

    public async Task<Notification> GetNotificationById(int notificationId)
    {
        var notification = await _context.Notifications
            .Include(n => n.ActorUser)
            .FirstOrDefaultAsync(n => n.Id == notificationId);
            
        if (notification == null)
            throw new InvalidOperationException($"Notification with ID {notificationId} not found.");
            
        return notification;
    }

    public async Task UpdateNotification(Notification notification)
    {
        if (notification == null)
            throw new ArgumentNullException(nameof(notification));

        _context.Notifications.Update(notification);
        await _context.SaveChangesAsync();
    }

    public async Task AddNotification(Notification notification)
    {
        if (notification == null)
            throw new ArgumentNullException(nameof(notification));

        await _context.Notifications.AddAsync(notification);
        await _context.SaveChangesAsync();
    }
    
    public async Task<(IEnumerable<Notification> notifications, int totalCount)> GetNotificationsByType(int userId, NotificationStatus status, int pageNumber, int pageSize)
    {
        var query = _context.Notifications
            .Include(n => n.ActorUser)
            .Where(n => n.UserId == userId && n.Type == status);
        
        var totalCount = await query.CountAsync();
        
        var notifications = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return (notifications, totalCount);
    }

    public async Task MarkNotificationAsRead(int notificationId)
    {
        var notification = await GetNotificationById(notificationId);
        notification.isRead = true;
        notification.UpdatedAt = DateTime.UtcNow;
        await UpdateNotification(notification);
    }

    public async Task MarkAllNotificationsAsRead(int userId)
    {
        await _context.Notifications
            .Where(n => n.UserId == userId && !n.isRead)
            .ExecuteUpdateAsync(n => n
                .SetProperty(x => x.isRead, true)
                .SetProperty(x => x.UpdatedAt, DateTime.UtcNow));
    }
}