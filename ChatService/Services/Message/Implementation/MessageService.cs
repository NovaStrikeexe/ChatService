using ChatService.Data;
using ChatService.Models;

namespace ChatService.Services.Message.Implementation;

public class MessageService(IMessageRepository messageRepository) : IMessageService
{
    public async Task<MsgDto> SaveMessageAsync(Msg msg) 
        => await messageRepository.SaveMessageAsync(msg);

    public async Task<IEnumerable<MsgDto>> GetMessagesAsync(DateTime startTime, DateTime endTime) 
        => await messageRepository.GetMessagesAsync(startTime, endTime);
}