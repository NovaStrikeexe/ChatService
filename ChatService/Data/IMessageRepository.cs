using ChatService.Models;

namespace ChatService.Data;

public interface IMessageRepository
{
    Task<MsgDto> SaveMessageAsync(Msg msg);
    
    Task<IEnumerable<MsgDto>> GetMessagesAsync(DateTime startTime, DateTime endTime);
}