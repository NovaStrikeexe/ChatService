using ChatService.Data.Contracts;

namespace ChatService.Data.MessageRepository;

public interface IMessageRepository
{
    Task SaveMessageAsync(MessageDataDto messageDataDto, CancellationToken cancellationToken);
    
    Task<IEnumerable<MessageDataDto>> GetMessagesAsync(DateTime startTime, DateTime endTime, CancellationToken cancellationToken);
}