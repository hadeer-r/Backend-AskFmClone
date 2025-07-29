namespace AskFm.DAL.Models;

public class Follow
{
    public int FollowerId { get; set; }
    public virtual User? Follower { get; set; }
    public int FollowedId { get; set; }

    public virtual User? Followed { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // is this follow available
    public bool IsActive { get; set; } = true;
}