namespace AskFm.DAL.Moodels;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Bio { get; set; }
    public string AvatarPath { get; set; }
    
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime LastSeen { get; set; }

    public virtual ICollection<Thread>? AskedThreads { get; set; }
    public virtual ICollection<Thread>? ReceivedThreads { get; set; }
    public virtual ICollection<Comment>? Comments { get; set; }
    public virtual ICollection<Follow>? Followers { get; set; }
    public virtual ICollection<Follow>? Following { get; set; }
    public virtual ICollection<ThreadLike>? ThreadLikes { get; set; }
    public virtual ICollection<CommentLike>? CommentLikes { get; set; }
    public virtual ICollection<Notification>? Notifications { get; set; }
    public virtual ICollection<SavedThreads>? SavedThreads { get; set; }
    
}