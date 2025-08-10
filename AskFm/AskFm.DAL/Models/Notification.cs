using System.Runtime.InteropServices.JavaScript;

namespace AskFm.DAL.Models;

public class Notification : ITrackable
{
    public GCNotificationStatus Type;
    public int Id { get; set; }
    public int UserId { get; set; }
    public virtual ApplicationUser? User { get; set; }
    public bool isRead { get; set; }
    
    public int ResourceId { get; set; }
    public string jsonContent { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime DeletedAt { get; set; }
}