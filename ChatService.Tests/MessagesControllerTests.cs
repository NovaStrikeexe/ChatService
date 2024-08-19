using ChatService.Controllers;
using ChatService.Models;
using ChatService.Services.Message;
using ChatService.Services.WebSocket;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace ChatService.Tests
{
    public class MessagesControllerTests
    {
        private readonly Mock<IMessageService> _messageService;
        private readonly MessagesController _controller;

        public MessagesControllerTests()
        {
            _messageService = new Mock<IMessageService>();
            Mock<ISignalRService> signalRService = new();
            Mock<ILogger<MessagesController>> logger = new();
            _controller = new MessagesController(_messageService.Object, signalRService.Object, logger.Object);
        }

        [Fact]
        public async Task PostMessage_ShouldReturnOk()
        {
            var message = new Msg { Id = 1, Content = "Test message"};
            var savedMessage = new MsgDto { Id = 1, Content = "Test message", Date = DateTime.UtcNow };
            _messageService.Setup(m => m.SaveMessageAsync(message)).ReturnsAsync(savedMessage);

            var result = await _controller.PostMessage(message);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task GetMessages_ShouldReturnOk()
        {
            var startTime = DateTime.UtcNow.AddHours(-1);
            var endTime = DateTime.UtcNow;
            var messages = new List<MsgDto> { new() { Id = 1, Content = "Test message", Date = DateTime.UtcNow } };
            _messageService.Setup(m => m.GetMessagesAsync(startTime, endTime)).ReturnsAsync(messages);

            var result = await _controller.GetMessages(startTime, endTime);

            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnedMessages = Assert.IsType<List<MsgDto>>(actionResult.Value);
            Assert.Single(returnedMessages);
        }
    }
}
