using AskFm.DAL.Moodels;
using Microsoft.EntityFrameworkCore;
using Thread = AskFm.DAL.Moodels.Thread;

namespace AskFm.DAL;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Thread> Threads { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Follow> Follows { get; set; }
    public DbSet<ThreadLike> ThreadLikes { get; set; }
    public DbSet<CommentLike> CommentLikes { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<SavedThreads> SavedThreads { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}