using AskFm.BLL.DTO.UserDTOs;
using AskFm.BLL.Services.UserIdentityService;
using AskFm.DAL;
using AskFm.DAL.Interfaces;
using AskFm.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Tests;

public class UserTests
{
    private UserService _userService;
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private Mock<UserManager<ApplicationUser>> _mockUserManager;
    private Mock<IApplicationUserRepository> _mockApplicationUserRepository;

    public UserTests()
    {
        // mock setup
        _mockUserManager = GetMockUserManager();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockApplicationUserRepository = new Mock<IApplicationUserRepository>();
        // uesr repo setup
        _mockUnitOfWork.Setup(u => u.Users).Returns(_mockApplicationUserRepository.Object);
        
        // service creation
        _userService = new UserService(_mockUnitOfWork.Object, _mockUserManager.Object, _mockHttpContextAccessor.Object);
        
        // user manager save
        _mockUserManager.Setup(um => um.UpdateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);

    }
    
    
    
    /// 
    /// Test updateUserAsync Service with 3 conditions
    /// 
    [Fact]
    public async Task UpdatedUser_UpdatedWithCorrectUserAndCorrectData_SuccessWithUserData()
    {
        // Arrange
        int userId = 1;
        var appUser = new ApplicationUser
        {
            Id = userId,
            Name = "OldName",
            Bio = "Old Bio",
            AvatarPath = "/old.jpg"
        };

        var updatedUser = new UpdateUserDTO
        {
            Name = "John",
            Bio = "this is John, software engineer.",
            AvatarPath = "/image.jpg"
        };

        // UnitOfWork
        _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(userId)).ReturnsAsync(appUser);
        _mockUnitOfWork.Setup(u => u.Users.UpdateAsync(It.IsAny<ApplicationUser>())).Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.FromResult(1));


        // Act
        var result = await _userService.UpdateUserAsync(userId, updatedUser);

        // Assert
        Assert.True(result.success);

        _mockUnitOfWork.Verify(u => u.Users.UpdateAsync(It.Is<ApplicationUser>(u => u.Id == userId)), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdatedUser_ThePassedUserIsNull_UpdateFaild()
    {
        
    }
    
    [Fact]
    public async Task UpdatedUser_ThePassedUserIsNotFound_UpdateFaild()
    {
        
    }

    /// 
    /// Update DeleteUserAsync with 2 conditions
    /// 
    [Fact]
    public async Task DeleteUserAsync_ThePassedUserNotFound_DeleteFaild()
    {
        
    }   
    [Fact]
    public async Task DeleteUserAsync_FoundedUser_DeleteSuccess()
    {
        
    }
    

    
    
    

    // helper functions
    private Mock<UserManager<ApplicationUser>> GetMockUserManager()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        var options = new Mock<IOptions<IdentityOptions>>();
        var passwordHasher = new Mock<IPasswordHasher<ApplicationUser>>();
        var userValidators = new List<IUserValidator<ApplicationUser>>();
        var passwordValidators = new List<IPasswordValidator<ApplicationUser>>();
        var keyNormalizer = new Mock<ILookupNormalizer>();
        var errors = new Mock<IdentityErrorDescriber>();
        var services = new Mock<IServiceProvider>();
        var logger = new Mock<ILogger<UserManager<ApplicationUser>>>();

        return new Mock<UserManager<ApplicationUser>>(
            store.Object,
            options.Object,
            passwordHasher.Object,
            userValidators,
            passwordValidators,
            keyNormalizer.Object,
            errors.Object,
            services.Object,
            logger.Object);

    }
}