namespace AskFm.DAL.Moodels;

public class ThreadLike
{
    public int ThreadId { get; set; }
    public Thread Thread { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public DateTime CreatedAt { get; set; }
}