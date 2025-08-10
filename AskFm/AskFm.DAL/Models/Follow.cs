using System.Runtime.InteropServices.JavaScript;

namespace AskFm.DAL.Models;

public class Follow : ITrackable
{
    public int FollowerId { get; set; }
    public virtual ApplicationUser? Follower { get; set; }
    public int FollowedId { get; set; }

    public virtual ApplicationUser? Followed { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // is this follow available
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }
    public DateTime DeletedAt { get; set; }
}