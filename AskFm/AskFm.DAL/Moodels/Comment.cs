namespace AskFm.DAL.Moodels;

public class Comment
{
    public int Id { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public int LikeCount { get; set; }

    public int? UserId { get; set; }
    public virtual User? User { get; set; }

    public int ThreadId { get; set; }
    public virtual Thread? Thread { get; set; }

    public int? ParentCommentId { get; }
    public virtual Comment? ParentComment { get; set; }

    public virtual ICollection<Comment>? Replies { get; set; }
    public virtual ICollection<CommentLike>? CommentLikes { get; set; }
}