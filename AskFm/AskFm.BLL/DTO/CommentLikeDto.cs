namespace AskFm.BLL.DTO;

public class CommentLikeDto
{
    public int CommentId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; }
    public DateTime CreatedAt { get; set; }
}