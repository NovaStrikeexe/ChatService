using ChatService.Models;

namespace ChatService.Services.WebSocket;

public interface IWebSocketService
{
    public Task HandleWebSocketAsync(HttpContext context);

    public Task SendMessageToAllAsync(Msg msg);
}