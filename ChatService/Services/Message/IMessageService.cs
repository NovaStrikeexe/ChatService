using ChatService.Models;

namespace ChatService.Services.Message;

public interface IMessageService
{
    public Task SaveMessageAsync(Msg msg);

    public Task<IEnumerable<Msg>> GetMessagesAsync(DateTime start, DateTime end);
}