using ChatService.Models;

namespace ChatService.Services.WebSocket;

public interface ISignalRService
{
    public Task SendMessageToAllAsync(MsgDto msgDto);
}