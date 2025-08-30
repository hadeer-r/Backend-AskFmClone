using System.Runtime.InteropServices.JavaScript;
using AskFm.BLL.DTO;
using AskFm.DAL.Interfaces;
using AskFm.DAL.Models;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;

namespace AskFm.BLL.Services;

public class CommentLikeService :  ICommentLikeService
{
    
    private IUnitOfWork _unitOfWork;
    private readonly ILogger<CommentLikeService> _logger;
    private readonly IMapper _mapper;
    public CommentLikeService(IUnitOfWork unitOfWork,  ILogger<CommentLikeService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    
    
    public async Task<ServiceResult<IEnumerable<CommentLikeDto>>> GetLikesForCommentAsync(int commentId)
    {
        try
        {
            _logger.LogInformation("Retrieving likes for comment id: {CommentId}", commentId);

            var comment = await _unitOfWork.Comments.GetByIdAsync(commentId);
            if (comment == null)
            {
                var errors = new List<String>()
                {
                    $"Comment with id {commentId} not found"
                };
                return await ServiceResult<IEnumerable<CommentLikeDto>>.Failure(errors);
            }

            var likes = await _unitOfWork.CommentLikes.FindAllAsync(
                predicate: cl => cl.CommentId == commentId && !cl.IsDeleted
            );

            var likeDtos = likes.Select(like => new CommentLikeDto
            {
                CommentId = like.CommentId,
                UserId = like.UserId,
                CreatedAt = like.CreatedAt,
                UserName = like.User?.UserName
            });

            return await ServiceResult<IEnumerable<CommentLikeDto>>.Success(likeDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving likes for comment id: {CommentId}", commentId);
            return await ServiceResult<IEnumerable<CommentLikeDto>>.Failure(new List<string>() { ex.Message });
        }    
    }

    public async Task<ServiceResult<CommentLikeDto>> AddLikeAsync(int commentId, int userId)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
            {
                _logger.LogInformation("Adding like for comment id: {CommentId} by user id: {UserId}", 
                    commentId, userId);

                var comment = await _unitOfWork.Comments.GetByIdAsync(commentId);
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    var errors = new List<String>()
                    {
                        $"User with id {userId} not found"
                    };
                    await transaction.RollbackAsync();
                    return await ServiceResult<CommentLikeDto>.Failure(errors);
                }
                
                string userName = user.UserName;
                if (comment == null)
                {
                    var errors = new List<String>()
                    {
                        $"Comment with id {commentId} not found"
                    };
                    await transaction.RollbackAsync();
                    return await ServiceResult<CommentLikeDto>.Failure(errors);
                }

                var existingLike = await _unitOfWork.CommentLikes.FindAsync(
                    cl => cl.CommentId == commentId && cl.UserId == userId
                );

                // if the CommentLike already exit in the DB
                if (existingLike != null)
                {
                    // if the The use already liked this comment
                    if (!existingLike.IsDeleted)
                    {
                        var errors = new List<String>()
                        {
                            "User has already liked this comment"
                        };
                        await transaction.RollbackAsync();
                        return await ServiceResult<CommentLikeDto>.Failure(errors);
                    }

                    // otherwise , the user liked the comment , then unliked it , and then wants to like it again
                    existingLike.IsDeleted = false;
                    comment.LikeCount++;
                    _unitOfWork.Comments.Update(comment);
                    await _unitOfWork.SaveAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation("Like added successfully for comment id: {CommentId}", commentId);
                    
                    
                    // updating the createdAt column to Now , ignoring the first time the user liked the comment
                    existingLike.CreatedAt = DateTime.Now;
                    return await ServiceResult<CommentLikeDto>.Success(new CommentLikeDto
                    {
                        CommentId = existingLike.CommentId,
                        UserId = existingLike.UserId,
                        UserName = userName,
                        CreatedAt = existingLike.CreatedAt
                    });
                }

                var newLike = new CommentLike
                {
                    CommentId = commentId,
                    UserId = userId,
                };

                await _unitOfWork.CommentLikes.AddAsync(newLike);
                
                comment.LikeCount++;
                _unitOfWork.Comments.Update(comment);

                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Like added successfully for comment id: {CommentId}", commentId);
                return await ServiceResult<CommentLikeDto>.Success(new CommentLikeDto
                {
                    CommentId = newLike.CommentId,
                    UserId = newLike.UserId,
                    UserName = userName,
                    CreatedAt = newLike.CreatedAt
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error adding like for comment id: {CommentId} by user id: {UserId}", 
                    commentId, userId);
                return await ServiceResult<CommentLikeDto>.Failure(new List<string>(){ex.Message});
            }
        
    }
    

    public async Task<ServiceResult<CommentLikeDto>> DeleteLikeAsync(int commentId, int userId)
    {
        var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            _logger.LogInformation("Deleting the comment like from user {userid} on comment id {commentId}", userId, commentId);
            
            var comment = await _unitOfWork.Comments.GetByIdAsync(commentId);

            if (comment == null)
            {
                var errors = new List<string>()
                {
                    "$User didn't like this comment"
                };
                await transaction.RollbackAsync();
                return await ServiceResult<CommentLikeDto>.Failure(errors);
            }
            
            
            var commentLike = await 
                _unitOfWork.CommentLikes.FindAsync(
                    predicate: cl => cl.CommentId == commentId && cl.UserId == userId && !cl.IsDeleted);
            
            // if the user didn't like  this comment before 
            if (commentLike == null)
            {
                var errors = new List<string>()
                {
                    "User didn't like this comment"
                    
                };
                await transaction.RollbackAsync();
                return await ServiceResult<CommentLikeDto>.Failure(errors);
            }
            
            await _unitOfWork.CommentLikes.RemoveAsync(commentLike);
            
            
            if (comment.CommentLikes != null)
                comment.CommentLikes.Remove(commentLike);
            
            if (comment.LikeCount > 0)
                    comment.LikeCount--;
            
            _unitOfWork.Comments.Update(comment);
            
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync();
            return await ServiceResult<CommentLikeDto>.Success();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            _logger.LogError(e, "Failed to delete like by user {UserId} on comment {CommentId}", userId, commentId);
            return await ServiceResult<CommentLikeDto>.Failure(new List<string>(){e.Message});
        }
    }
}