using System.Net.Http.Json;
using ChatService.MessageSenderClient.Shared.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace ChatService.MessageSenderClient.Services.Http.Implementation
{
    public class MessageService(HttpClient httpClient) : IMessageService
    {
        public async Task<bool> SendMessageAsync(MsgDto message)
        {
            var response = await httpClient.PostAsJsonAsync("api/v1/message/send-message", message);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<MsgDto>> GetMessageHistoryAsync(DateTime startTime, DateTime endTime)
            => await httpClient.GetFromJsonAsync<List<MsgDto>>($"/api/v1/message/get-messages?startTime={startTime:o}&endTime={endTime:o}");
    }
}