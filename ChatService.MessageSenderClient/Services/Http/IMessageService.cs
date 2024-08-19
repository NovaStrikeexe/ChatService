using ChatService.MessageSenderClient.Shared.Models;

namespace ChatService.MessageSenderClient.Services.Http
{
    public interface IMessageService
    {
        Task<bool> SendMessageAsync(MsgDto message);
        Task<List<MsgDto>> GetMessageHistoryAsync(DateTime startTime, DateTime endTime);
    }
}