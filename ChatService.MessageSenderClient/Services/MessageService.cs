using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using ChatService.MessageSenderClient.Models;

namespace ChatService.MessageSenderClient.Services;

public class MessageService(HttpClient httpClient)
{
    private readonly ClientWebSocket _webSocket = new();
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public event Action<Message>? OnMessageReceived;

    public async Task SendMessageAsync(Message message)
    {
        await httpClient.PostAsJsonAsync("api/v1/messages", message);
    }

    public async Task<IEnumerable<Message>> GetMessagesAsync(DateTime startTime, DateTime endTime)
    {
        var response = await httpClient.GetAsync($"api/v1/messages?startTime={startTime:O}&endTime={endTime:O}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<Message>>();
    }

    public async Task StartWebSocket()
    {
        try
        {
            await _webSocket.ConnectAsync(new Uri("ws://localhost:5294/api/v1/ws"), _cancellationTokenSource.Token);
            var buffer = new byte[8192];

            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellationTokenSource.Token);

                switch (result.MessageType)
                {
                    case WebSocketMessageType.Text:
                    {
                        var messageJson = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        var message = JsonSerializer.Deserialize<Message>(messageJson);
                        OnMessageReceived?.Invoke(message);
                        break;
                    }
                    case WebSocketMessageType.Close:
                        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", _cancellationTokenSource.Token);
                        break;
                    case WebSocketMessageType.Binary:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WebSocket exception: {ex.Message}");
        }
    }

    public void StopWebSocket()
    {
        _cancellationTokenSource.Cancel();
        _webSocket.Dispose();
    }
}