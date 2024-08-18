using ChatService.MessageSenderClient.Shared.Models;

namespace ChatService.MessageSenderClient.Services
{
    public interface IMessageService
    {
        Task<bool> SendMessageAsync(MsgDto message);
        Task StartListeningAsync(Action<MsgDto> onMessageReceived);
        Task<List<MsgDto>> GetMessageHistoryAsync(DateTime startTime, DateTime endTime);
    }
}