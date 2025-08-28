using AskFm.DAL.Models;

namespace AskFm.BLL.Services;

public interface ICommentService
{
    Task<ServiceResult<Comment>> GetCommentAsync(int commentId);
}