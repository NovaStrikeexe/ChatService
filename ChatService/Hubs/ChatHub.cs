using Microsoft.AspNetCore.SignalR;
using ChatService.Models;

namespace ChatService.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessageToAllAsync(MsgDto msgDto)
        {
            await Clients.All.SendAsync("ReceiveMessage", msgDto);
        }
    }
}