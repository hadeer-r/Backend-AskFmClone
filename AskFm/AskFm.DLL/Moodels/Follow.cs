using System.ComponentModel.DataAnnotations;

namespace AskFm.DLL.Moodels;

public class Follow
{
    public int FollowerId { get; set; }
    public User Follower  { get; set; }
    public int FollowedId { get; set; }
    
    public User Followed { get; set; }
    public int FollwerCount { get; set; }
    public int FollwingCount { get; set; }
    public DateTime CreatedAt { get; set; }
}