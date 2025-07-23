namespace AskFm.DLL.Moodels;

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
    public int FolloweingCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastSeen { get; set; }
    
    public ICollection<Thread> AskedThreads { get; set; }
    public ICollection<Thread> ReceivedThreads { get; set; }
    public ICollection<Comment> Comments { get; set; }
    public ICollection<Follow> Followers { get; set; }
    public ICollection<Follow> Following { get; set; }
    public ICollection<ThreadLike> QuestionLikes { get; set; }
    public ICollection<CommentLike> CommentLikes { get; set; }
    public ICollection<Notification> Notifications { get; set; }
    
}