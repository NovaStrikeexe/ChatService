using ChatService.Controllers;
using ChatService.Services.Message;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ChatService.Contracts.Http;
using ChatService.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Tests
{
    public class MessagesControllerTests
    {
        private readonly Mock<IMessageService> _messageService;
        private readonly Mock<IHubContext<ChatHub>> _hubContext;
        private readonly Mock<ILogger<MessagesController>> _logger;
        private readonly MessagesController _controller;

        public MessagesControllerTests()
        {
            _messageService = new Mock<IMessageService>();
            _hubContext = new Mock<IHubContext<ChatHub>>();
            _logger = new Mock<ILogger<MessagesController>>();
            _controller = new MessagesController(_messageService.Object, _hubContext.Object, _logger.Object);
        }

        [Fact]
        public async Task PostMessage_ShouldReturnOk()
        {
            // Arrange
            var data =  DateTime.UtcNow;
            var message = new MessageDto { Id = 1, Content = "Test message" , Date = data};
            var savedMessage = new MessageDto { Id = 1, Content = "Test message", Date = data};

            MessageRequest messageRequest = new MessageRequest
            {
                Id = message.Id,
                Content = message.Content
            };
            _messageService.Setup(m => m.SaveMessageAsync(message, CancellationToken.None))
                .ReturnsAsync(savedMessage);

            // Act
            var result = await _controller.PostMessage(messageRequest, CancellationToken.None);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task GetMessages_ShouldReturnOk()
        {
            // Arrange
            var startTime = DateTime.UtcNow.AddHours(-1);
            var endTime = DateTime.UtcNow;
            var messages = new List<MessageDto> { new() { Id = 1, Content = "Test message", Date = DateTime.UtcNow } };
            
            _messageService.Setup(m => m.GetMessagesAsync(startTime, endTime, CancellationToken.None))
                .ReturnsAsync(messages);

            // Act
            var result = await _controller.GetMessages(startTime, endTime, CancellationToken.None);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnedMessages = Assert.IsType<List<MessageDto>>(actionResult.Value);
            Assert.Single(returnedMessages);
        }
    }
}
