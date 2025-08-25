using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AskFm.BLL.DTO.UserDTOs;
using AskFm.DAL;
using AskFm.DAL.Interfaces;
using AskFm.DAL.Models;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AskFm.BLL.Services.UserIdentityService;

public class AuthService : IAuthService
{
    private IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtOptions _jwtOptions;

    public AuthService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IOptions<JwtOptions> jwtOptions)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<ServiceResult<AuthResponseDTO>> LoginAsync(LoginDTO request)
    {
        if (request == null)
        {
            var erros = new List<string>
            {
                "Invalid Email or Password."
            };
            return await ServiceResult<AuthResponseDTO>.Failure(erros);
        }

        var getUser = await _userManager.FindByEmailAsync(request.Email);

        if (getUser == null)
        {
            var erros = new List<string>
            {
                "Invalid Email or Password."
            };
            return await ServiceResult<AuthResponseDTO>.Failure(erros);
        }

        var passwordValid = await _userManager.CheckPasswordAsync(getUser, request.Password);
        if (!passwordValid)
        {
            var errors = new List<string> { "Invalid Email or Password." };
            return await ServiceResult<AuthResponseDTO>.Failure(errors);
        }

        var response = await GetAuthToken(getUser);
        return response;

    }
    

    public async Task<ServiceResult<AuthResponseDTO>> RegisterAsync(RegisterUserDTO request)
    {
        if (request == null)
        {
            var errors = new List<string>{ "Invalid Request Data" };
            return await ServiceResult<AuthResponseDTO>.Failure(errors);
        }
        var oldUser = _userManager.FindByEmailAsync(request.Email).Result;
        if (oldUser != null)
        {
            var errors = new List<string>{ "Email already exist" };
            return await ServiceResult<AuthResponseDTO>.Failure(errors);
        }
        
        var newUser = new ApplicationUser()
        {
            Name = request.Name,
            Email = request.Email,
            UserName = request.Username,
            Bio = request.Bio,
            AvatarPath = request.AvatarPath,
            LastSeen = DateTime.UtcNow
        };
        var createRsult = await _userManager.CreateAsync(newUser,request.Passwrod);
        

        if (createRsult.Succeeded == false)
        {
            
            var errors = createRsult.Errors.Select(e => e.Description).ToList();
            return await ServiceResult<AuthResponseDTO>.Failure(errors);
            
            
        }

        var response = await GetAuthToken(newUser);
        return response;
    }

    public async Task<ServiceResult<AuthResponseDTO>> RefreshTokenAsync(int id, string refreshToken)
    {
        if (refreshToken == null)
        {
            var errors = new List<string> { "Invalid Token." };
            return await ServiceResult<AuthResponseDTO>.Failure(errors);
        }

        var user = _unitOfWork.Users.GetById(id);
        if (user == null)
        {
            var errors = new List<string> { "Invalid User." };
            return await ServiceResult<AuthResponseDTO>.Failure(errors);
        }
        if (!user.RefreshTokens.Any(r => r.Token == refreshToken))
        {
            var errors = new List<string> { "Invalid Token." };
            return await ServiceResult<AuthResponseDTO>.Failure(errors);
        }
        
        var oldRefreshToken = user.RefreshTokens.Single(t => t.Token == refreshToken);
        if (!oldRefreshToken.IsActive)
        {
            var errors = new List<string> { "InActive Token." };
            return await ServiceResult<AuthResponseDTO>.Failure(errors);
        }

        oldRefreshToken.RevokedOn = DateTime.UtcNow;

        return await GetAuthToken(user);

    }

    
    public async Task<ServiceResult<bool>> RevokeRefreshTokenAsync(int id, string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            var errors = new List<string> { "Invalid Token." };
            return await ServiceResult<bool>.Failure(errors);
        }

        var user = _unitOfWork.Users.GetById(id);
        if (user == null)
        {
            var errors = new List<string> { "Invalid User." };
            return await ServiceResult<bool>.Failure(errors);
        }
        
        if (!user.RefreshTokens.Any(r => r.Token == refreshToken))
        {
            var errors = new List<string> { "Invalid Token." };
            return await ServiceResult<bool>.Failure(errors);
        }
        
        var oldRefreshToken = user.RefreshTokens.Single(t => t.Token == refreshToken);
        
        if (!oldRefreshToken.IsActive)
        {
            var errors = new List<string> { "InActive Token." };
            return await ServiceResult<bool>.Failure(errors);
        }

        oldRefreshToken.RevokedOn = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);

        return await ServiceResult<bool>.Success(true);

    }

    public void Logout()
    {
        

    }


    private async Task<ServiceResult<AuthResponseDTO>> GetAuthToken(ApplicationUser user)
    {
        var token = await GenerateJwtToken(user);
        if (string.IsNullOrEmpty(token))
        {
            var errors = new List<string> { "Invalid Data" };
            return await ServiceResult<AuthResponseDTO>.Failure(errors);
        }

        RefreshToken refreshToken = null;
        if (user.RefreshTokens !=null  && user.RefreshTokens.Any(r => r.IsActive))
        {
            refreshToken = user.RefreshTokens.FirstOrDefault(r => r.IsActive);
        }
        else
        {
            refreshToken = await generateRefreshToken();
            user.RefreshTokens ??= new List<RefreshToken>();
            user.RefreshTokens.Add(refreshToken);
        }
        
        user.LastSeen = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        return await ServiceResult<AuthResponseDTO>.Success(new AuthResponseDTO()
        {
            Token = token,
            RefreshToken = refreshToken,
            IsAuthenticated = true,
            User = new ReadUserDTO
            {
                Name = user.Name,
                Email = user.Email,
                LastSeen = user.LastSeen,
                Bio = user.Bio,
                AvatarPath = user.AvatarPath,
                followerCount = user.FollowersCount
            }
        });
    }
    private async Task<RefreshToken> generateRefreshToken()
    {
        var randomNumber = new byte[32];

        using var generator = new RNGCryptoServiceProvider();

        generator.GetBytes(randomNumber);

        return new RefreshToken
        {
            Token = Convert.ToBase64String(randomNumber),
            ExpireOn = DateTime.UtcNow.AddDays(10),
            CreatedOn = DateTime.UtcNow
        };

    }
    private Task<string> GenerateJwtToken(ApplicationUser appUser)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor(){
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessExpiration),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey)), SecurityAlgorithms.HmacSha256),
            Subject = new ClaimsIdentity(new Claim[] 
            {
                new(ClaimTypes.Name, appUser.Name),
                new(ClaimTypes.Email, appUser.Email),
                new("UserId", appUser.Id.ToString())
            })
        };
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(securityToken);
        return Task.FromResult(accessToken);
    }
}

