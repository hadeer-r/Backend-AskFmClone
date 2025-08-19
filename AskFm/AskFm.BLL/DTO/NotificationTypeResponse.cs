namespace AskFm.BLL.DTO;

public class NotificationTypeResponse
{
    public string Type { get; set; }
    public List<NotificationDto> Notifications { get; set; }
    public PaginationDto Pagination { get; set; }
}