using ChatService.Contracts.Http;

namespace ChatService.Data.MessageRepository;

public interface IMessageRepository
{
    Task<MessageDto> SaveMessageAsync(MessageDto messageDto, CancellationToken cancellationToken);
    
    Task<IEnumerable<MessageDto>> GetMessagesAsync(DateTime startTime, DateTime endTime, CancellationToken cancellationToken);
}