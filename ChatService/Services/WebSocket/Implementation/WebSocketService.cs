using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using ChatService.Models;

namespace ChatService.Services.WebSocket.Implementation;

public class WebSocketService : IWebSocketService
{
    private readonly List<System.Net.WebSockets.WebSocket> _sockets = [];
    
    public async Task SendMessageToAllAsync(Msg msg)
    {
        var messageBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(msg));
        var tasks = _sockets.Select(socket => socket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None));

        await Task.WhenAll(tasks);
    }
    
    public async Task HandleWebSocketAsync(HttpContext context)
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            var socket = await context.WebSockets.AcceptWebSocketAsync();
            _sockets.Add(socket);

            await ListenAsync(socket);
        }
    }

    private async Task ListenAsync(System.Net.WebSockets.WebSocket socket)
    {
        var buffer = new byte[1024 * 4];
        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType != WebSocketMessageType.Close) continue;
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by the WebSocketManager", CancellationToken.None);
            _sockets.Remove(socket);
        }
    }
}