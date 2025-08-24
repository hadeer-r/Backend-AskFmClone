using AskFm.BLL.DTO;

namespace AskFm.BLL.Services;

public interface ICommentLikeService
{
    Task<IEnumerable<CommentLikeDto>> GetLikesForCommentAsync(int commentId);
    Task<CommentLikeDto> AddLikeAsync(int commentId, int userId);
    Task DeleteLikeAsync(int commentId, int userId);
}