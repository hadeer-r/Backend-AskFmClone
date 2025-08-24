using AskFm.DAL.Models;

namespace AskFm.BLL.Services;

public interface ICommentService
{
    Comment GetComment(int commentId);
}