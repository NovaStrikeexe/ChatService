using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using ChatService.Models;

namespace ChatService.Services.WebSocket.Implementation;

public class WebSocketService : IWebSocketService
{
    private readonly ILogger<WebSocketService> _logger;
    private readonly List<System.Net.WebSockets.WebSocket> _sockets = new();

    public WebSocketService(ILogger<WebSocketService> logger)
    {
        _logger = logger;
    }

    public async Task SendMessageToAllAsync(MsgDto msgDto)
    {
        _logger.LogInformation($"{nameof(WebSocketService)}: {nameof(SendMessageToAllAsync)}: Sending message to all clients - Id: {msgDto.Id}, Content: {msgDto.Content}, Date: {msgDto.Date}");

        var messageBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(msgDto));
        var tasks = _sockets.Select(socket => socket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None));

        try
        {
            await Task.WhenAll(tasks);
            _logger.LogInformation($"{nameof(WebSocketService)}: {nameof(SendMessageToAllAsync)}: Message successfully sent to all clients.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(WebSocketService)}: {nameof(SendMessageToAllAsync)}: Error occurred while sending message to all clients.");
            throw;
        }
    }

    public async Task HandleWebSocketAsync(HttpContext context)
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            _logger.LogInformation($"{nameof(WebSocketService)}: {nameof(HandleWebSocketAsync)}: WebSocket request received.");

            var socket = await context.WebSockets.AcceptWebSocketAsync();
            _sockets.Add(socket);

            _logger.LogInformation($"{nameof(WebSocketService)}: {nameof(HandleWebSocketAsync)}: WebSocket accepted. Total sockets: {_sockets.Count}");

            await ListenAsync(socket);
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            _logger.LogWarning($"{nameof(WebSocketService)}: {nameof(HandleWebSocketAsync)}: Invalid WebSocket request.");
        }
    }

    private async Task ListenAsync(System.Net.WebSockets.WebSocket socket)
    {
        var buffer = new byte[1024 * 4];
        while (socket.State == WebSocketState.Open)
        {
            try
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    _logger.LogInformation($"{nameof(WebSocketService)}: {nameof(ListenAsync)}: WebSocket closing. Removing socket.");
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by the WebSocketManager", CancellationToken.None);
                    _sockets.Remove(socket);
                    _logger.LogInformation($"{nameof(WebSocketService)}: {nameof(ListenAsync)}: WebSocket closed and removed.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(WebSocketService)}: {nameof(ListenAsync)}: Error occurred while listening to WebSocket.");
                throw;
            }
        }
    }
}