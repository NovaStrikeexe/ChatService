using ChatService.Configuration.Models;
using ChatService.Contracts.Http;
using ChatService.Data.DataConnect;
using ChatService.Data.MessageRepository.Implementation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace ChatService.Tests;

public class MessageRepositoryTests
{
    private readonly MessageRepository _repository;

    public MessageRepositoryTests()
    {
        var loggerMock = new Mock<ILogger<MessageRepository>>();
        var dataConnectMock = new Mock<IDataConnect>();
            
        var dbSettings = new Mock<IOptions<DbSettings>>();
        dbSettings.Setup(ds => ds.Value).Returns(new DbSettings { DefaultConnection = "YourConnectionString" }); 
        dataConnectMock
            .Setup(dc => dc.ExecuteQueryAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Dictionary<string, object>>
            {
                new()
                {
                    { "id", 1 },
                    { "content", "Test message" },
                    { "date", DateTime.UtcNow }
                }
            });

        dataConnectMock
            .Setup(dc => dc.ExecuteScalarAsync<int>(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _repository = new MessageRepository(loggerMock.Object, dbSettings.Object, dataConnectMock.Object);
    }

    [Fact]
    public async Task SaveMessageAsync_ShouldSaveMessage()
    {
        var message = new MessageDto
        {
            Id = 1,
            Content = "Test message",
            Date = DateTime.UtcNow
        };

        var result = await _repository.SaveMessageAsync(message, new CancellationToken());

        Assert.NotNull(result);
        Assert.Equal(message.Id, result.Id);
        Assert.Equal(message.Content, result.Content);
    }

    [Fact]
    public async Task GetMessagesAsync_ShouldReturnMessages()
    {
        var startTime = DateTime.UtcNow.AddHours(-1);
        var endTime = DateTime.UtcNow;

        var messages = await _repository.GetMessagesAsync(startTime, endTime, new CancellationToken());

        Assert.NotNull(messages);
        Assert.IsType<List<MessageDto>>(messages);
        Assert.Single(messages);
    }
}