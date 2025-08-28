using AskFm.BLL.DTO;

namespace AskFm.BLL.Services;

public interface ICommentLikeService
{
    Task<ServiceResult<IEnumerable<CommentLikeDto>>> GetLikesForCommentAsync(int commentId);
    Task<ServiceResult<CommentLikeDto>> AddLikeAsync(int commentId, int userId);
    Task<ServiceResult<CommentLikeDto>> DeleteLikeAsync(int commentId, int userId);
}