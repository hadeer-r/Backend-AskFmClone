using AskFm.BLL.DTO;
using AskFm.BLL.Hub;
using AskFm.DAL.Enums;
using AskFm.DAL.Interfaces;
using AskFm.DAL.Models;
using Microsoft.AspNetCore.Mvc.Diagnostics;
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
        public async Task GetNotificationsByType_WithINValidCategory_ReturnsFilteredNotifications()
        {
            // Arrang
            int userId = 1;
            string category = "InvalidCategory";
            // Act and Assert

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _notificationService.GetNotificationsByType(userId, category));
            Assert.Contains("Invalid notification category", ex.Message);
        }

        [Fact]
        public async Task MarkNotificationAsRead_WithValidId_UpdatesNotification()
        {
            // Arrange

            int notificationId = 1;
            Notification notification = new Notification { Id = notificationId, UserId = 1, IsRead = false };
            _unitOfWorkMock.Setup(p => p.Notifications.GetByIdAsync(notificationId)).ReturnsAsync(notification);
            _unitOfWorkMock.Setup(p => p.SaveAsync()).ReturnsAsync(1);

            // Act
            var result = await _notificationService.MarkNotificationAsRead(notificationId);

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
            _unitOfWorkMock.Setup(p => p.Notifications.GetByIdAsync(notificationId)).ReturnsAsync((Notification)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _notificationService.MarkNotificationAsRead(notificationId));

            Assert.Contains("Notification with ID 999 not found", ex.Message);
        }

        [Fact]
        public async Task MarkAllNotificationsAsRead_UpdatesAllUnreadNotifications()
        {
            // Arrange
            var userId = 1;
            var unreadNotifications = new List<Notification>
            {
                new Notification { Id = 1, UserId = userId, IsRead = false },
                new Notification { Id = 2, UserId = userId, IsRead = false }
            };

            _unitOfWorkMock.Setup(p => p.Notifications.FindAllAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Notification, bool>>>(), It.IsAny<string[]>()))
                .ReturnsAsync(unreadNotifications);
            _unitOfWorkMock.Setup(p => p.SaveAsync()).ReturnsAsync(1);

            // Act
            var result = await _notificationService.MarkAllNotificationsAsRead(userId);

            // Assert
            Assert.Equal("All notifications marked as read", result);
            Assert.True(unreadNotifications.All(n => n.IsRead));
            _unitOfWorkMock.Verify(p => p.Notifications.Update(It.IsAny<Notification>()), Times.Exactly(2));
            _unitOfWorkMock.Verify(p => p.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateNotification_WithValidData_CreatesNotificationAndSendsSignalR()
        {
            // Arrange
            var userId = 1;
            var type = NotificationStatus.QUESTION;
            var resourceId = 100;
            var message = "Test notification message";
            var actorUser = new ApplicationUser
            {
                Id = 2,
                UserName = "test_actor",
                AvatarPath = "actor.jpg"
            };

            // Setup specific mock behavior for this test
            _mockClients.Setup(p => p.Group($"user_{userId}")).Returns(_mockClientProxy.Object);

            _unitOfWorkMock.Setup(p => p.Notifications.AddAsync(It.IsAny<Notification>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(p => p.SaveAsync()).ReturnsAsync(1);

            _notificationRepositoryMock.Setup(r => r.GetActorUserByResourceId(resourceId, type))
                .ReturnsAsync(actorUser);

            // Act
            await _notificationService.CreateNotification(userId, type, resourceId, message);

            // Assert
            // Verify notification was added to database
            _unitOfWorkMock.Verify(p => p.Notifications.AddAsync(It.Is<Notification>(n =>
                n.UserId == userId &&
                n.Type == type &&
                n.ResourceId == resourceId &&
                n.Message == message &&
                !n.IsRead &&
                n.CreatedAt != default &&
                n.UpdatedAt != default
            )), Times.Once);

            // Verify database save was called
            _unitOfWorkMock.Verify(p => p.SaveAsync(), Times.Once);

            // Verify actor user was fetched
            _notificationRepositoryMock.Verify(r => r.GetActorUserByResourceId(resourceId, type), Times.Once);

            // Verify SignalR notification was sent to correct group
            _mockClientProxy.Verify(cp => cp.SendCoreAsync(
                "ReceiveNotification",
                It.Is<object[]>(args => args.Length == 1 &&
                    ((NotificationDto)args[0]).UserId == userId &&
                    ((NotificationDto)args[0]).Type == type.ToString() &&
                    ((NotificationDto)args[0]).Message == message &&
                    ((NotificationDto)args[0]).Actor.Username == "test_actor" &&
                    ((NotificationDto)args[0]).Actor.AvatarPath == "actor.jpg"
                ),
                default(CancellationToken)
            ), Times.Once);
        }

        [Fact]
        public async Task CreateNotification_WhenDatabaseSaveFails_ThrowsException()
        {
            // Arrange
            var userId = 1;
            var type = NotificationStatus.ANSWER;
            var resourceId = 300;
            var message = "Answer notification";

            _unitOfWorkMock.Setup(p => p.Notifications.AddAsync(It.IsAny<Notification>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(p => p.SaveAsync())
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _notificationService.CreateNotification(userId, type, resourceId, message)
            );

            Assert.Equal("Database error", exception.Message);

            // Verify notification was attempted to be added
            _unitOfWorkMock.Verify(p => p.Notifications.AddAsync(It.IsAny<Notification>()), Times.Once);
            _unitOfWorkMock.Verify(p => p.SaveAsync(), Times.Once);
        }
    }
    
    
}