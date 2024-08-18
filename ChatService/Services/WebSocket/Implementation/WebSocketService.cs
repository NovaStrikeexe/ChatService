using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using ChatService.Models;

namespace ChatService.Services.WebSocket.Implementation;

public class WebSocketService(ILogger<WebSocketService> logger) : IWebSocketService
{
    private readonly List<System.Net.WebSockets.WebSocket> _sockets = [];
    private readonly object _syncLock = new();
    public async Task SendMessageToAllAsync(MsgDto msgDto)
    {
        logger.LogInformation($"{nameof(WebSocketService)}: {nameof(SendMessageToAllAsync)}: Sending message to all clients - Id: {msgDto.Id}, Content: {msgDto.Content}, Date: {msgDto.Date}");

        var messageBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(msgDto));
        List<Task> tasks;

        lock (_syncLock)
        {
            tasks = _sockets.Select(socket => socket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None)).ToList();
        }

        try
        {
            await Task.WhenAll(tasks);
            logger.LogInformation($"{nameof(WebSocketService)}: {nameof(SendMessageToAllAsync)}: Message successfully sent to all clients.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"{nameof(WebSocketService)}: {nameof(SendMessageToAllAsync)}: Error occurred while sending message to all clients.");
            throw;
        }
    }

    public async Task HandleWebSocketAsync(HttpContext context)
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            logger.LogInformation($"{nameof(WebSocketService)}: {nameof(HandleWebSocketAsync)}: WebSocket request received.");

            var socket = await context.WebSockets.AcceptWebSocketAsync();
            lock (_syncLock)
            {
                _sockets.Add(socket);
            }

            logger.LogInformation($"{nameof(WebSocketService)}: {nameof(HandleWebSocketAsync)}: WebSocket accepted. Total sockets: {_sockets.Count}");

            await ListenAsync(socket);
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            logger.LogWarning($"{nameof(WebSocketService)}: {nameof(HandleWebSocketAsync)}: Invalid WebSocket request.");
        }
    }

    private async Task ListenAsync(System.Net.WebSockets.WebSocket socket)
    {
        var buffer = new byte[1024 * 4];
        try
        {
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType != WebSocketMessageType.Close) continue;
                logger.LogInformation($"{nameof(WebSocketService)}: {nameof(ListenAsync)}: WebSocket closing. Removing socket.");
                lock (_syncLock)
                {
                    _sockets.Remove(socket);
                }
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by the WebSocketService", CancellationToken.None);
                logger.LogInformation($"{nameof(WebSocketService)}: {nameof(ListenAsync)}: WebSocket closed and removed.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"{nameof(WebSocketService)}: {nameof(ListenAsync)}: Error occurred while listening to WebSocket. Removing socket.");
            lock (_syncLock)
            {
                _sockets.Remove(socket);
            }
            await socket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Error occurred", CancellationToken.None);
        }
        finally
        {
            socket.Dispose();
        }
    }
}
