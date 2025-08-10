using AskFm.DAL.Interfaces;
using AskFm.DAL.Models;
using AskFm.DAL.Repositories;
using Thread = System.Threading.Thread;

namespace AskFm.DAL;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    
    public IRepository<User> Users { get; private set; }
    public IRepository<Thread> Threads { get; private set; }
    public IRepository<SavedThreads> SavedThreads { get; private set; }
    public IRepository<ThreadLike> ThreadLikes { get; private set; }
    public IRepository<Comment> Comments { get; private set; }
    public IRepository<CommentLike> CommentLikes { get; private set; }
    public IRepository<Follow> Follows { get; private set; }
    public IRepository<Notification> Notifications { get; private set; }

    
    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        
        Users = new Repository<User>(_context);
        Threads = new Repository<Thread>(_context);
        SavedThreads = new Repository<SavedThreads>(_context);
        ThreadLikes = new Repository<ThreadLike>(_context);
        Comments = new Repository<Comment>(_context);
        CommentLikes = new Repository<CommentLike>(_context);
        Follows = new Repository<Follow>(_context);
        Notifications = new Repository<Notification>(_context);
        
    }
    
    
    public void Dispose()
    {
        _context.Dispose();
    }
    
    public int Complete()
    {
        return _context.SaveChanges();
    }

    public Task<int> CompleteAsync()
    {
        return _context.SaveChangesAsync();
    }
}