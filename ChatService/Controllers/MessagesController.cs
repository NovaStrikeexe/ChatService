using ChatService.Models;
using ChatService.Services.Message;
using ChatService.Services.WebSocket;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController(
    IMessageService messageService,
    IWebSocketService webSocketService,
    ILogger<MessagesController> logger)
    : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> PostMessage([FromBody] Msg msg)
    {
        try
        {
            logger.LogInformation("Received a new message to save: {Content}", msg.Content);
                
            //var ms = await messageService.SaveMessageAsync(msg); 
            //await webSocketService.SendMessageToAllAsync(ms);

            logger.LogInformation("Message saved and sent to clients: {Content}", msg.Content);
                
            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while saving the message.");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetMessages([FromQuery] DateTime startTime, [FromQuery] DateTime endTime)
    {
        try
        {
            logger.LogInformation("Fetching messages between {StartTime} and {EndTime}", startTime, endTime);
                
            var messages = await messageService.GetMessagesAsync(startTime, endTime);
                
            logger.LogInformation("Fetched {Count} messages", messages.LongCount());

            return Ok(messages);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while fetching messages.");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}