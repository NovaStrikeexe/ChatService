using System.Net.Http.Json;
using ChatService.MessageSenderClient.Services;
using ChatService.MessageSenderClient.Shared.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace ClientApp.Services
{
    public class MessageService : IMessageService
    {
        private readonly HttpClient _httpClient;
        private HubConnection _hubConnection;

        public MessageService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> SendMessageAsync(MsgDto message)
        {
            var response = await _httpClient.PostAsJsonAsync("api/v1/message/send-message", message);
            return response.IsSuccessStatusCode;
        }

        public async Task StartListeningAsync(Action<MsgDto> onMessageReceived)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5150/chatHub")
                .Build();

            _hubConnection.On<MsgDto>("ReceiveMessage", onMessageReceived);
            await _hubConnection.StartAsync();
        }

        public async Task<List<MsgDto>> GetMessageHistoryAsync(DateTime startTime, DateTime endTime)
            => await _httpClient.GetFromJsonAsync<List<MsgDto>>($"/api/v1/message/get-messages?startTime={startTime:o}&endTime={endTime:o}");
    }
}