namespace AskFm.DAL.Models;

public class ThreadLike
{
    public int ThreadId { get; set; }
    public virtual Thread? Thread { get; set; }

    public  int UserId { get; set; }
    public virtual User? User { get; set; }

    public DateTime CreatedAt { get; set; }
}