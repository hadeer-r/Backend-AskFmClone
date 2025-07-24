namespace AskFm.DAL.Moodels;

public class Follow
{
    public int FollowerId { get; set; }
    public User Follower { get; set; }
    public int FollowedId { get; set; }

    public User Followed { get; set; }
    public DateTime CreatedAt { get; set; }
}