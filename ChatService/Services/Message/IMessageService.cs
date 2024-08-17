using ChatService.Models;

namespace ChatService.Services.Message;

public interface IMessageService
{
    public Task<MsgDto> SaveMessageAsync(Msg msg);

    public Task<IEnumerable<MsgDto>> GetMessagesAsync(DateTime start, DateTime end);
}