using AskFm.BLL.DTO;
using AskFm.BLL.Hub;
using AskFm.BLL.Services;
using AskFm.DAL.Enums;
using AskFm.DAL.Interfaces;
using AskFm.DAL.Models;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Xunit;

namespace AskFm.BLL.Tests.Services
{
    public class NotificationServiceTests
    {
        private readonly Mock<INotificationRepository> _notificationRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IHubContext<NotificationHub>> _hubContextMock;
        private readonly Mock<IHubClients> _mockClients;
        private readonly Mock<IClientProxy> _mockClientProxy;
        private readonly Mock<IGroupManager> _mockGroups;
        private readonly NotificationService _notificationService;

        public NotificationServiceTests()
        {
            _notificationRepositoryMock = new Mock<INotificationRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _hubContextMock = new Mock<IHubContext<NotificationHub>>();
            _mockClients = new Mock<IHubClients>();
            _mockClientProxy = new Mock<IClientProxy>();
            _mockGroups = new Mock<IGroupManager>();

            // Setup hub context relationships
            _hubContextMock.Setup(p => p.Clients).Returns(_mockClients.Object);
            _hubContextMock.Setup(p => p.Groups).Returns(_mockGroups.Object);
            _mockClients.Setup(p => p.Group(It.IsAny<string>())).Returns(_mockClientProxy.Object);

            // Setup unit of work notifications repository mock
            var notificationRepoMock = new Mock<IRepository<Notification>>();
            _unitOfWorkMock.Setup(p => p.Notifications).Returns(notificationRepoMock.Object);

            _notificationService = new NotificationService(
                _notificationRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _hubContextMock.Object
            );
        }

        [Fact]
        public async Task GetUserNotifications_ReturnsCorrectDtoWithPagination()
        {
            // Arrange
            int userId = 1;
            int pageNumber = 1;
            int pageSize = 10;
            var notifications = new List<Notification>
            {
                new Notification
                {
                    Id = 1,
                    UserId = userId,
                    Type = NotificationStatus.QUESTION,
                    ResourceId = 100,
                    Message = "Test notification",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                }
            };

            var totalCount = 1;
            var actorUser = new ApplicationUser { Id = 2, UserName = "test_user", AvatarPath = "test.jpg" };

            _notificationRepositoryMock.Setup(p => p.GetAllNotifications(userId, pageNumber, pageSize)).ReturnsAsync((notifications, totalCount));
            _notificationRepositoryMock.Setup(p => p.GetActorUserByResourceId(100, NotificationStatus.QUESTION)).ReturnsAsync(actorUser);

            // Act
            var result = await _notificationService.GetUserNotifications(userId, pageNumber, pageSize);

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result[0].Id);
            Assert.Equal(NotificationStatus.QUESTION.ToString(), result[0].Type);
            Assert.Equal("Test notification", result[0].Message);
            Assert.False(result[0].IsRead);
            Assert.Equal("test_user", result[0].Actor.Username);
            Assert.Equal("test.jpg", result[0].Actor.AvatarPath);
            Assert.Equal(2, result[0].Actor.Id);
            Assert.Equal(1, result[0].Pagination.TotalCount);
        }

        [Fact]
        public async Task GetUserNotifications_WithNullActor_ReturnsCorrectDtoWithPagination()
        {
            // Arrange
            int userId = 1;
            int pageNumber = 1;
            int pageSize = 10;
            var notifications = new List<Notification>
            {
                new Notification
                {
                    Id = 1,
                    UserId = userId,
                    Type = NotificationStatus.QUESTION,
                    ResourceId = 100,
                    Message = "Test notification",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                }
            };

            var totalCount = 1;

            _notificationRepositoryMock.Setup(p => p.GetAllNotifications(userId, pageNumber, pageSize)).ReturnsAsync((notifications, totalCount));
            _notificationRepositoryMock.Setup(p => p.GetActorUserByResourceId(100, NotificationStatus.QUESTION)).ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _notificationService.GetUserNotifications(userId, pageNumber, pageSize);

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result[0].Id);
            Assert.Equal(NotificationStatus.QUESTION.ToString(), result[0].Type);
            Assert.Equal("Test notification", result[0].Message);
            Assert.False(result[0].IsRead);
            Assert.Null(result[0].Actor);
            Assert.Equal(1, result[0].Pagination.TotalCount);
        }

        [Fact]
        public async Task GetNotificationsByType_WithValidCategory_ReturnsFilteredNotifications()
        {
            // Arrange
            int userId = 1;
            string category = "answer";
            int pageNumber = 1;
            int pageSize = 10;
            var notifications = new List<Notification>
            {
                new Notification
                {
                    Id = 1,
                    UserId = userId,
                    Type = NotificationStatus.ANSWER,
                    ResourceId = 100,
                    Message = "Test notification",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                }
            };

            var totalCount = 1;
            var actorUser = new ApplicationUser { Id = 2, UserName = "test_user", AvatarPath = "test.jpg" };

            _notificationRepositoryMock.Setup(p => p.GetNotificationsByType(userId, NotificationStatus.ANSWER, pageNumber, pageSize)).ReturnsAsync((notifications, totalCount));
            _notificationRepositoryMock.Setup(p => p.GetActorUserByResourceId(100, NotificationStatus.ANSWER)).ReturnsAsync(actorUser);

            // Act
            var result = await _notificationService.GetNotificationsByType(userId, category, pageNumber, pageSize);

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result[0].Id);
            Assert.Equal(category.ToUpper(), result[0].Type);
            Assert.Equal("Test notification", result[0].Message);
            Assert.False(result[0].IsRead);
            Assert.Equal("test_user", result[0].Actor.Username);
            Assert.Equal("test.jpg", result[0].Actor.AvatarPath);
            Assert.Equal(2, result[0].Actor.Id);
            Assert.Equal(1, result[0].Pagination.TotalCount);
        }

        [Fact]
        public async Task GetNotificationsByType_WithInvalidCategory_ThrowsArgumentException()
        {
            // Arrange
            int userId = 1;
            string category = "InvalidCategory";
            int pageNumber = 1;
            int pageSize = 10;

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _notificationService.GetNotificationsByType(userId, category, pageNumber, pageSize));
            Assert.Contains("Invalid notification category", ex.Message);
        }

        [Fact]
        public async Task MarkNotificationAsRead_WithValidId_UpdatesNotification()
        {
            // Arrange
            int notificationId = 1;
            int userId = 1;
            Notification notification = new Notification 
            { 
                Id = notificationId, 
                UserId = userId, 
                IsRead = false,
                Type = NotificationStatus.QUESTION,
                ResourceId = 100,
                Message = "Test notification",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _notificationRepositoryMock.Setup(p => p.GetUserNotificationById(notificationId, userId))
                .ReturnsAsync(notification);
            _unitOfWorkMock.Setup(p => p.SaveAsync()).ReturnsAsync(1);

            // Act
            var result = await _notificationService.MarkNotificationAsRead(notificationId, userId);

            // Assert
            Assert.True(notification.IsRead);
            Assert.Equal("notification has been read", result);
            _unitOfWorkMock.Verify(p => p.Notifications.Update(notification), Times.Once);
            _unitOfWorkMock.Verify(p => p.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task MarkNotificationAsRead_WithInvalidId_ThrowsInvalidOperationException()
        {
            // Arrange
            var notificationId = 999;
            var userId = 1;
            _notificationRepositoryMock.Setup(p => p.GetUserNotificationById(notificationId, userId))
                .ReturnsAsync((Notification)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _notificationService.MarkNotificationAsRead(notificationId, userId));

            Assert.Contains("Notification not found or access denied", ex.Message);
        }

        [Fact]
        public async Task MarkAllNotificationsAsRead_UpdatesAllUnreadNotifications()
        {
            // Arrange
            int userId = 1;
            var unreadNotifications = new List<Notification>
            {
                new Notification 
                { 
                    Id = 1, 
                    UserId = userId, 
                    IsRead = false,
                    Type = NotificationStatus.QUESTION,
                    ResourceId = 100,
                    Message = "Test notification 1",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Notification 
                { 
                    Id = 2, 
                    UserId = userId, 
                    IsRead = false,
                    Type = NotificationStatus.ANSWER,
                    ResourceId = 200,
                    Message = "Test notification 2",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            _unitOfWorkMock.Setup(p => p.Notifications.FindAllAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Notification, bool>>>(), null))
                .ReturnsAsync(unreadNotifications);
            _unitOfWorkMock.Setup(p => p.SaveAsync()).ReturnsAsync(1);

            // Act
            var result = await _notificationService.MarkAllNotificationsAsRead(userId);

            // Assert
            Assert.Equal("All notifications marked as read", result);
            Assert.All(unreadNotifications, n => Assert.True(n.IsRead));
            _unitOfWorkMock.Verify(p => p.Notifications.Update(It.IsAny<Notification>()), Times.Exactly(2));
            _unitOfWorkMock.Verify(p => p.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateNotification_CreatesAndSendsNotification()
        {
            // Arrange
            int userId = 1;
            var type = NotificationStatus.FOLLOW;
            int resourceId = 100;
            string message = "Test notification";
            var actorUser = new ApplicationUser { Id = 2, UserName = "test_user", AvatarPath = "test.jpg" };

            _unitOfWorkMock.Setup(p => p.Notifications.AddAsync(It.IsAny<Notification>()));
            _unitOfWorkMock.Setup(p => p.SaveAsync()).ReturnsAsync(1);
            _notificationRepositoryMock.Setup(p => p.GetActorUserByResourceId(resourceId, type))
                .ReturnsAsync(actorUser);

            // Act
            await _notificationService.CreateNotification(userId, type, resourceId, message);

            // Assert
            _unitOfWorkMock.Verify(p => p.Notifications.AddAsync(It.IsAny<Notification>()), Times.Once);
            _unitOfWorkMock.Verify(p => p.SaveAsync(), Times.Once);
            _mockClientProxy.Verify(p => p.SendCoreAsync("ReceiveNotification", It.IsAny<object[]>(), default), Times.Once);
        }
    }
}