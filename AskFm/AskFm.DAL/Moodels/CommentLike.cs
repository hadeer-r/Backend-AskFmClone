namespace AskFm.DAL.Moodels;

public class CommentLike
{
    public int CommentId { get; set; }
    public virtual Comment? Comment { get; set; }
    public int UserId { get; set; }
    public virtual User? User { get; set; }
    public DateTime CreatedAt { get; set; }
}