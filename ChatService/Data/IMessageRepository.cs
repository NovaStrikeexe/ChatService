using ChatService.Models;

namespace ChatService.Data;

public interface IMessageRepository
{
    Task SaveMessageAsync(Msg msg);
    Task<IEnumerable<Msg>> GetMessagesAsync(DateTime startTime, DateTime endTime);
}