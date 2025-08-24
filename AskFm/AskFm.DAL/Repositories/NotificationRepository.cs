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
        var query = _context.Notifications.Where(n => n.UserId == userId);
            
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
        var notification = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId);
            
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
        var query = _context.Notifications.Where(n => n.UserId == userId && n.Type == status);
        
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
        //notification.UpdatedAt = DateTime.UtcNow;
        await UpdateNotification(notification);
    }

    public async Task MarkAllNotificationsAsRead(int userId)
    {
        await _context.Notifications
            .Where(n => n.UserId == userId && !n.isRead)
            .ExecuteUpdateAsync(n => n
                .SetProperty(x => x.isRead, true));
                // .SetProperty(x => x.UpdatedAt, DateTime.UtcNow))
    }

    public async Task<ApplicationUser?> GetActorUserByResourceId(int resourceId, NotificationStatus type)
    {
        // In follow case the follow model does not have a follow id so we use the followedId to get the actor user
        if (type == NotificationStatus.FOLLOW)
        {
            return await _context.Follows
                .Where(f => f.FollowedId == resourceId)
                .Select(f => f.Follower)
                .FirstOrDefaultAsync();
        }
        else if (type == NotificationStatus.QUESTION)
        {
            return await _context.Threads
                .Where(t => t.Id == resourceId)
                .Select(t => t.Asker)
                .FirstOrDefaultAsync();
        }
        else if (type == NotificationStatus.ANSWER)
        {
            return await _context.Threads
                .Where(t => t.Id == resourceId)
                .Select(t => t.Asked)
                .FirstOrDefaultAsync();
        }
        else if (type == NotificationStatus.COMMENT_LIKE)
        {
            return await _context.CommentLikes
                .Where(cl => cl.CommentId == resourceId)
                .Select(cl => cl.User)
                .FirstOrDefaultAsync();
        }
        else if (type == NotificationStatus.QUESTION_LIKE)
        {
            return await _context.ThreadLikes
                .Where(tl => tl.ThreadId == resourceId)
                .Select(tl => tl.User)
                .FirstOrDefaultAsync();
        }
        else if (type == NotificationStatus.REPLAY)
        {
            return await _context.Comments
                .Where(c => c.Id == resourceId)
                .Select(c => c.User)
                .FirstOrDefaultAsync();
        }
        return null;

    }
}