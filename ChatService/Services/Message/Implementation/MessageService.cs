using ChatService.Data;
using ChatService.Models;

namespace ChatService.Services.Message.Implementation;

public class MessageService(IMessageRepository messageRepository) : IMessageService
{
    public async Task SaveMessageAsync(Msg msg) 
        => await messageRepository.SaveMessageAsync(msg);

    public async Task<IEnumerable<Msg>> GetMessagesAsync(DateTime startTime, DateTime endTime) 
        => await messageRepository.GetMessagesAsync(startTime, endTime);
}