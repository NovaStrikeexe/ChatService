using ChatService.Configuration.Models;
using ChatService.Data.Implementation;
using ChatService.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace ChatService.Tests;

public class MessageRepositoryTests
{
    private readonly MessageRepository _repository;

    public MessageRepositoryTests()
    {
        Mock<ILogger<MessageRepository>> logger = new();
        Mock<IOptions<DbSettings>> dbSettings = new();
        dbSettings.Setup(m => m.Value).Returns(new DbSettings { DefaultConnection = "Host=localhost;Port=5432;Username=user;Password=password;Database=chatservice" });
        _repository = new MessageRepository(logger.Object, dbSettings.Object);
    }

    [Fact]
    public async Task SaveMessageAsync_ShouldSaveMessage()
    {
        var message = new Msg { Id = 1, Content = "Test message"};

        var result = await _repository.SaveMessageAsync(message);

        Assert.NotNull(result);
        Assert.Equal(message.Id, result.Id);
        Assert.Equal(message.Content, result.Content);
    }

    [Fact]
    public async Task GetMessagesAsync_ShouldReturnMessages()
    {
        var startTime = DateTime.UtcNow.AddHours(-1);
        var endTime = DateTime.UtcNow;
            
        var messages = await _repository.GetMessagesAsync(startTime, endTime);

        Assert.NotNull(messages);
        Assert.IsType<List<MsgDto>>(messages);
    }
}