using AskFm.BLL.DTO;

public class NotificationDto
{
    public int Id { get; set; }
    public string Type { get; set; }

    public string Message { get; set; }

    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }

    public int ResourceId { get; set; }
    public int UserId { get; set; }
    public ActorDto? Actor { get; set; }
    public PaginationDto Pagination { get; set; }


}
