using AskFm.BLL.DTO;
using AskFm.BLL.Services;
using AskFm.DAL.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AskFm.API.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentController : ControllerBase
{
    
    private readonly ICommentLikeService _commentLikeService;
    private readonly ICommentService _commentService;
    private readonly ILogger<CommentController> _logger;
    
    
    public CommentController(
        ICommentLikeService commentLikeService,
        ICommentService commentService,
        ILogger<CommentController> logger)
    {
        _commentLikeService = commentLikeService;
        _logger = logger;
        _commentService = commentService;
    }
    
    
    
    
    
    // GET api/comment/{id}/likes -> get all the likes for a Comment with id = id
    [HttpGet("{id}/likes")]
    public async Task<IActionResult> GetAllLikes(int id)
    {
        try
        {
            var likes = await _commentLikeService.GetLikesForCommentAsync(id);
            return Ok(likes);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Comment not found with id: {CommentId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving likes for comment id: {CommentId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving likes" });
        }
    }
    
    
    
    
    // POST api/comment/{id}/likes -> add a like for a Comment with id = id
    [HttpPost("{id}/likes")]
    public async Task<IActionResult> AddLike(int id, [FromBody] CreateCommentLikeDto likeDto)
    {
        try
        {
            var userId = likeDto.UserId;
            
            var createdLike = await _commentLikeService.AddLikeAsync(id, userId);
            
            return CreatedAtAction(
                nameof(GetAllLikes), 
                new { id = id }, 
                createdLike);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid request to add like to comment id: {CommentId}", id);
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot add like to comment id: {CommentId}", id);
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding like to comment id: {CommentId}", id);
            return StatusCode(500, new { message = "An error occurred while adding like" });
        }
    }
    
    
    
    
    [HttpDelete("{id}/likes")]
    public async Task<IActionResult> DeleteLike(int id, int userId)
    {
        try
        {
            var comment = _commentService.GetComment(id);
            if (comment == null)
                throw new ArgumentException($"Comment with id {id} not found");
            
            if (userId <= 0)
                return BadRequest(new { message = "Invalid user id." });
            
            await _commentLikeService.DeleteLikeAsync(id, userId);
            
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Like not found for comment id: {CommentId} and user {UserId}", id, userId);
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized delete attempt for comment id: {CommentId} by user {UserId}", id, userId);
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting like from comment id: {CommentId} by user {UserId}", id, userId);
            return StatusCode(500, new { message = "An error occurred while deleting like" });
        }
    }
    
}