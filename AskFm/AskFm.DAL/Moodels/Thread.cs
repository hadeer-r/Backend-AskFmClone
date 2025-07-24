using AskFm.DAL.Enums;
namespace AskFm.DAL.Moodels;

public class Thread
{
    public int Id { get; set; }
    public string QuestionContent { get; set; }
    public string AnswerContent { get; set; }

    public ThreadStatus Status { get; set; }
    public bool isAnonymous { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? AskerId { get; set; }
    public virtual User? Asker { get; set; }

    public int AskedId { get; set; }
    public virtual User? Asked { get; set; }

    public virtual ICollection<Comment>? Comments { get; set; }
    public virtual ICollection<ThreadLike>? ThreadLikes { get; set; }
}