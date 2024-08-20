using ChatService.Data.Contracts;
using ChatService.Data.DataConnect;
using ChatService.Data.MessageRepository.Implementation;
using Microsoft.Extensions.Logging;
using Moq;

namespace ChatService.Tests;

public class MessageRepositoryTests
{
    private readonly MessageRepository _repository;
    private readonly Mock<IDataConnect> _dataConnectMock;
    private readonly Mock<ILogger<MessageRepository>> _loggerMock;

    public MessageRepositoryTests()
    {
        _loggerMock = new Mock<ILogger<MessageRepository>>();
        _dataConnectMock = new Mock<IDataConnect>();

        _dataConnectMock
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

        _repository = new MessageRepository(_loggerMock.Object, _dataConnectMock.Object);
    }

    [Fact]
    public async Task SaveMessageAsync_ShouldSaveMessage()
    {
        var messageDataDto = new MessageDataDto
        {
            Id = 1,
            Content = "Test message",
            Date = DateTime.UtcNow
        };

        await _repository.SaveMessageAsync(messageDataDto, new CancellationToken());

        _dataConnectMock.Verify(
            dc => dc.ExecuteQueryAsync(It.IsAny<string>(), It.Is<Dictionary<string, object>>(p =>
                (int)p["@Id"] == messageDataDto.Id &&
                (string)p["@Content"] == messageDataDto.Content &&
                (DateTime)p["@Date"] == messageDataDto.Date), It.IsAny<CancellationToken>()), Times.Once);

        _loggerMock.Verify(
            l => l.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Warning),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("No rows returned from SaveMessageAsync")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
    }

    [Fact]
    public async Task GetMessagesAsync_ShouldReturnMessages()
    {
        var startTime = DateTime.UtcNow.AddHours(-1);
        var endTime = DateTime.UtcNow;

        var messages = await _repository.GetMessagesAsync(startTime, endTime, new CancellationToken());

        Assert.NotNull(messages);
        Assert.IsType<List<MessageDataDto>>(messages);
        Assert.Single(messages);

        var message = messages.First();
        Assert.Equal(1, message.Id);
        Assert.Equal("Test message", message.Content);
    }

    [Fact]
    public async Task GetMessagesAsync_ShouldLogErrorOnException()
    {
        _dataConnectMock
            .Setup(dc => dc.ExecuteQueryAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Test exception"));

        var startTime = DateTime.UtcNow.AddHours(-1);
        var endTime = DateTime.UtcNow;

        await Assert.ThrowsAsync<Exception>(() => _repository.GetMessagesAsync(startTime, endTime, new CancellationToken()));

        _loggerMock.Verify(
            l => l.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An error occurred while retrieving messages")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
    }
}
