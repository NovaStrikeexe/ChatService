using ChatService.Contracts.Http;

namespace ChatService.Services.Message
{
    public interface IMessageService
    {
        Task<MessageDto> SaveMessageAsync(MessageDto message, CancellationToken cancellationToken);
        Task<IEnumerable<MessageDto>> GetMessagesAsync(DateTime startTime, DateTime endTime, CancellationToken cancellationToken);
    }
}