using ChatService.SignalR.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.SignalR.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessageToAllAsync(MessageSignalRDto messageDto)
        {
            await Clients.All.SendAsync("ReceiveMessage", messageDto);
        }
    }
}