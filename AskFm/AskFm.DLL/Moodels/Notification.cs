namespace AskFm.DLL.Moodels;

public class Notification
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }

    public GCNotificationStatus Type;
    public bool isRead { get; set; }
    public int ResourceId { get; set; }
    public string jsonContent { get; set; }
    public DateTime CreatedAt { get; set; }
}