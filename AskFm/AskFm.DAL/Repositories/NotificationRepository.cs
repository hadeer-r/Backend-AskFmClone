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
    
    public async Task<IEnumerable<Notification>> GetAllNotifications(int userId, int pageNumber, int pageSize)
    {
        var totalCount = await _context.Notifications.CountAsync(n => n.UserId == userId);
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return notifications;
    }

    public async Task<Notification> GetNotificationById(int notificationId)
    {
        var notification = await _context.Notifications.FindAsync(notificationId);
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
    public Task<IQueryable<Notification>> GetNotificationsByType(int userId, NotificationStatus status, int pageNumber, int pageSize)
    {
        var query = _context.Notifications.AsQueryable();

        if (userId > 0)
        {
            query = query.Where(n => n.UserId == userId);
        }
        
        query = query.Where(n => n.Type == status);

        return Task.FromResult(query.OrderByDescending(n => n.CreatedAt)
                 .Skip((pageNumber - 1) * pageSize)
                 .Take(pageSize));
    }
}