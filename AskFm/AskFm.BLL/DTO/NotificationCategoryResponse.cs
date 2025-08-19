namespace AskFm.BLL.DTO;

public class NotificationCategoryResponse
{
    public string Category { get; set; }
    public List<NotificationDto> Notifications { get; set; }
    public PaginationDto Pagination { get; set; }
}