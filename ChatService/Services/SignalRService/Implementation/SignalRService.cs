using ChatService.Hubs;
using ChatService.Models;
using ChatService.Services.WebSocket;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Services.SignalRService.Implementation
{
    public class SignalRService(IHubContext<ChatHub> hubContext, ILogger<SignalRService> logger)
        : ISignalRService
    {
        public async Task SendMessageToAllAsync(MsgDto msgDto)
        {
            logger.LogInformation($"{nameof(SignalRService)}: {nameof(SendMessageToAllAsync)}: Sending message to all clients - Id: {msgDto.Id}, Content: {msgDto.Content}, Date: {msgDto.Date}");

            try
            {
                await hubContext.Clients.All.SendAsync("ReceiveMessage", msgDto);
                logger.LogInformation($"{nameof(SignalRService)}: {nameof(SendMessageToAllAsync)}: Message successfully sent to all clients.");
            }
            catch (Exception exception)
            {
                logger.LogError(exception, $"{nameof(SignalRService)}: {nameof(SendMessageToAllAsync)}: Error occurred while sending message to all clients.");
                throw;
            }
        }
    }
}