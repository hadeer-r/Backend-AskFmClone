using AskFm.BLL.DTO.UserDTOs;
using AskFm.BLL.Services.UserIdentityService;
using AskFm.DAL;
using AskFm.DAL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AskFm.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class UserController : ControllerBase
{
    private IUnitOfWork _unitOfWork;
    private IAuthService  _authService;
    public IUserService _userService;

    public UserController(IUnitOfWork unitOfWork, IAuthService authService, IUserService userService)
    {
        _unitOfWork = unitOfWork;
        _authService = authService;
        _userService = userService;
    }
    [HttpGet]
    [Route("GetUsers")]
    public async Task<IActionResult> GetUsers()
    {
        var result = _unitOfWork.Users.GetAll().Select(u => new
        {
            name = u.Name,
            email = u.Email,
            username = u.UserName,
            bio = u.Bio,
        }).ToList();
        return Ok(result);
    }

    [HttpGet]
    [Route("current-user")]
    public async Task<IActionResult> GetCurrentUserAsync()
    {
        var result = await _userService.GetCurrentUserAsync();

        if (!result.success)
        {
            return BadRequest(result.Errors);
        }
        return Ok(result.Data);
    }
    /*
  GET Users only for now
    getUserbyId
    EditUser
    DeleteUser
    FollowUser
    unfollowUser
    reset password
    confirm email
    Helper Function: getCurrentUserId
  */   
    
}