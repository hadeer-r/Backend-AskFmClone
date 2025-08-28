using AskFm.DAL.Interfaces;
using AskFm.DAL.Models;

namespace AskFm.BLL.Services;

public class CommentService : ICommentService
{
    private IUnitOfWork _unitOfWork;
    public CommentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<ServiceResult<Comment>> GetCommentAsync(int commentId)
    {
        try
        {
            var comment = await _unitOfWork.Comments.GetByIdAsync(commentId);
            return await ServiceResult<Comment>.Success(comment);
        }
        catch (Exception e)
        {
            return await ServiceResult<Comment>.Failure(new List<string>(){e.Message});
        }
    }

    
}