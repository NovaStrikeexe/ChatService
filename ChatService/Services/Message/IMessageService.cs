using ChatService.Http.Contracts;

namespace ChatService.Services.Message
{
    public interface IMessageService
    {
        Task SaveMessageAsync(MessageDto message, CancellationToken cancellationToken);
        Task<IEnumerable<MessageDto>> GetMessagesAsync(DateTime startTime, DateTime endTime, CancellationToken cancellationToken);
    }
}