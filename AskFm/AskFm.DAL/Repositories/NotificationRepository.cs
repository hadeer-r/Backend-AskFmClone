using AskFm.DAL.Enums;
using AskFm.DAL.Interfaces;
using AskFm.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace AskFm.DAL.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public NotificationRepository(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<(IEnumerable<Notification> notifications, int totalCount)> GetAllNotifications(int userId, int pageNumber, int pageSize)
    {
        var query = _unitOfWork.Notifications.FindAll(n => n.UserId == userId);

        var totalCount = await query.CountAsync();

        var notifications = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (notifications, totalCount);
    }

    public async Task<(IEnumerable<Notification> notifications, int totalCount)> GetNotificationsByType(int userId, NotificationStatus status, int pageNumber, int pageSize)
    {
        var query = _unitOfWork.Notifications.FindAll(n => n.UserId == userId && n.Type == status);

        var totalCount = await query.CountAsync();

        var notifications = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (notifications, totalCount);
    }

    public async Task<ApplicationUser?> GetActorUserByResourceId(int resourceId, NotificationStatus type)
    {
        if (type == NotificationStatus.FOLLOW)
        {
            var follow = await _unitOfWork.Follows.FindAsync(f => f.FollowedId == resourceId, new[] { "Follower" });
            return follow?.Follower;
        }
        else if (type == NotificationStatus.QUESTION)
        {
            var thread = await _unitOfWork.Threads.FindAsync(t => t.Id == resourceId, new[] { "Asker" });
            return thread?.Asker;
        }
        else if (type == NotificationStatus.ANSWER)
        {
            var thread = await _unitOfWork.Threads.FindAsync(t => t.Id == resourceId, new[] { "Asked" });
            return thread?.Asked;
        }
        else if (type == NotificationStatus.COMMENT_LIKE)
        {
            var commentLike = await _unitOfWork.CommentLikes.FindAsync(cl => cl.CommentId == resourceId, new[] { "User" });
            return commentLike?.User;
        }
        else if (type == NotificationStatus.QUESTION_LIKE)
        {
            var threadLike = await _unitOfWork.ThreadLikes.FindAsync(tl => tl.ThreadId == resourceId, new[] { "User" });
            return threadLike?.User;
        }
        else if (type == NotificationStatus.REPLAY)
        {
            var comment = await _unitOfWork.Comments.FindAsync(c => c.Id == resourceId, new[] { "User" });
            return comment?.User;
        }
        return null;
    }

    public async Task<Notification?> GetUserNotificationById(int notificationId, int userId)
    {
        return await _unitOfWork.Notifications.FindAsync(n => n.Id == notificationId && n.UserId == userId);
    }
}