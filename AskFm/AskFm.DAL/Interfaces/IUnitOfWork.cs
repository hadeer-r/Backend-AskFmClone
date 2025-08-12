using AskFm.DAL.Models;
using Thread = System.Threading.Thread;

namespace AskFm.DAL.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<User> Users { get; }
    IRepository<Thread>  Threads { get; }
    IRepository<SavedThreads> SavedThreads { get; }
    IRepository<ThreadLike>  ThreadLikes { get; }
    IRepository<Comment>  Comments { get; }
    IRepository<CommentLike>  CommentLikes { get; }
    IRepository<Follow>  Follows { get; }
    IRepository<Notification>  Notifications { get; }
    
    int Save();
    Task<int> SaveAsync();
    
}