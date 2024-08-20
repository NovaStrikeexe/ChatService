using Microsoft.AspNetCore.SignalR;
using ChatService.Contracts.SignalR;

namespace ChatService.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessageToAllAsync(MessageSignalRDto messageDto)
        {
            await Clients.All.SendAsync("ReceiveMessage", messageDto);
        }
    }
}