using AskFm.BLL.DTO.UserDTOs;
using AskFm.BLL.Services;
using AskFm.BLL.Services.UserIdentityService;
using AskFm.DAL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sprache;

namespace AskFm.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private IAuthService  _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> RegisterUser(RegisterUserDTO registerUser)
    {
        if (registerUser == null)
        {
            return BadRequest(new List<String>{"Invalid data"});
        }
        ServiceResult<AuthResponseDTO> result = await _authService.RegisterAsync(registerUser);
        if (!result.success)
        {
            return BadRequest(result);
        }
        setRefreshToken(result.Data.RefreshToken.Token,result.Data.RefreshToken.ExpireOn);
        return Ok(result);
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(LoginDTO login)
    {
        if (login == null)
        {
            return BadRequest(new List<String>{"Invalid data"});
        }
        ServiceResult<AuthResponseDTO> result = await _authService.LoginAsync(login);
        if (!result.success)
        {
            return BadRequest(result);
        }
        if (!string.IsNullOrEmpty(result.Data.Token))
        {
            setRefreshToken(result.Data.RefreshToken.Token,result.Data.RefreshToken.ExpireOn);
        }
        return Ok(result);
    }

    [HttpGet]
    [Route("refresh-token/{id}")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> RefreshToken(int id)
    {
        string refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized("Invalid Token");
        }
        ServiceResult<AuthResponseDTO> result = await _authService.RefreshTokenAsync(id,refreshToken);

        if (!result.success)
        {
            return BadRequest(result);
        }

        return Ok(result);

    }
    
    [HttpPost("logout/{id}")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> Logout(int id)
    {
        string refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
        {
            return BadRequest("token Is required");
        }
        
        ServiceResult<bool> result = await _authService.RevokeRefreshTokenAsync(id,refreshToken);

        if (!result.success)
        {
            return BadRequest(result);
        }
        

        return Ok(result);
    }
    
    private void setRefreshToken(string refreshToken,DateTime expires)
    {
        var cookieOption = new CookieOptions()
        {
            HttpOnly = true,
            Expires = expires.ToLocalTime()
        };
       Response.Cookies.Append("refreshToken", refreshToken, cookieOption);
    }
    
}