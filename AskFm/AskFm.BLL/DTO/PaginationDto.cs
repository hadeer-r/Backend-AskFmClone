namespace AskFm.BLL.DTO;

public class PaginationDto
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public bool HasNext { get; set; }
    public bool HasPrevious { get; set; }
}