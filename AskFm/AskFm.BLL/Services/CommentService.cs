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
    
    public Comment GetComment(int commentId)
    {
        var comment = _unitOfWork.Comments.GetById(commentId);
        return comment;
    }

    
}