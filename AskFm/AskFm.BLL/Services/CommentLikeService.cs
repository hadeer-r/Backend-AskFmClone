using AskFm.BLL.DTO;
using AskFm.DAL.Interfaces;
using AskFm.DAL.Models;
using AutoMapper;
using Microsoft.Extensions.Logging;

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
    
    
    
    public async Task<IEnumerable<CommentLikeDto>> GetLikesForCommentAsync(int commentId)
    {
        try
        {
            _logger.LogInformation("Retrieving likes for comment id: {CommentId}", commentId);

            var comment = await _unitOfWork.Comments.GetByIdAsync(commentId);
            if (comment == null)
            {
                throw new ArgumentException($"Comment with id {commentId} not found");
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

            return likeDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving likes for comment id: {CommentId}", commentId);
            throw;
        }    
    }

    public async Task<CommentLikeDto> AddLikeAsync(int commentId, int userId)
    {
        try
            {
                _logger.LogInformation("Adding like for comment id: {CommentId} by user id: {UserId}", 
                    commentId, userId);

                var comment = await _unitOfWork.Comments.GetByIdAsync(commentId);
                
                if (comment == null)
                {
                    throw new ArgumentException($"Comment with id {commentId} not found");
                }

                var existingLike = await _unitOfWork.CommentLikes.FindAsync(
                    cl => cl.CommentId == commentId && cl.UserId == userId && !cl.IsDeleted
                );

                if (existingLike != null)
                {
                    throw new InvalidOperationException("User has already liked this comment");
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

                _logger.LogInformation("Like added successfully for comment id: {CommentId}", commentId);

                
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                
                if (user == null)
                    throw new ArgumentException($"User with id {userId} not found");
                
                string userName = user.UserName;
                
                
                return new CommentLikeDto
                {
                    CommentId = newLike.CommentId,
                    UserId = newLike.UserId,
                    UserName = userName,
                    CreatedAt = newLike.CreatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding like for comment id: {CommentId} by user id: {UserId}", 
                    commentId, userId);
                throw;
            }
        
    }
    

    public async Task DeleteLikeAsync(int commentId, int userId)
    {
        try
        {
            _logger.LogInformation("Deleting the comment like from user {userid} on comment id {commentId}", userId, commentId);
            
            var comment = await _unitOfWork.Comments.GetByIdAsync(commentId);
            
            if(comment == null)
                throw new ArgumentException($"User didn't like this comment");
            
            
            var commentLike = await _unitOfWork.CommentLikes.FindAsync(cl => cl.CommentId == commentId && cl.UserId == userId && !cl.IsDeleted);
            
            // if the user didn't like  this comment before 
            if(commentLike == null)
                throw new ArgumentException($"User didn't like this comment");
            
            await _unitOfWork.CommentLikes.RemoveAsync(commentLike);
            
            
            if (comment.CommentLikes != null)
                comment.CommentLikes.Remove(commentLike);
            
            if (comment.LikeCount > 0)
                    comment.LikeCount--;
            
           _unitOfWork.Comments.Update(comment);
            
            await _unitOfWork.SaveAsync();
            
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to delete like by user {UserId} on comment {CommentId}", userId, commentId);
            throw;
        }
    }
}