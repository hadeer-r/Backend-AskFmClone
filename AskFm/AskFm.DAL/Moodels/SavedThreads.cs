namespace AskFm.DAL.Moodels;

public class SavedThreads
{
    public int SavedThreadId { get; set; }
    public int UserId { get; set; }
    
    public virtual Thread? Thread { get; set; }
    public virtual User? User { get; set; }
}