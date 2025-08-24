using AskFm.DAL.Models;

namespace AskFm.DAL.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<ApplicationUser> Users { get; }
    IRepository<Models.Thread>  Threads { get; }
    IRepository<SavedThreads> SavedThreads { get; }
    IRepository<ThreadLike>  ThreadLikes { get; }
    IRepository<Comment>  Comments { get; }
    IRepository<CommentLike>  CommentLikes { get; }
    IRepository<Follow>  Follows { get; }
    IRepository<Notification>  Notifications { get; }
    
    int Save();
    Task<int> SaveAsync();
    
}