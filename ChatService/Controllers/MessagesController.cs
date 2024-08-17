using ChatService.Models;
using ChatService.Services.Message;
using ChatService.Services.WebSocket;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ChatService.Controllers;

[ApiController]
[Route("api/v1/message")]
public class MessagesController(
    IMessageService messageService,
    IWebSocketService webSocketService,
    ILogger<MessagesController> logger)
    : ControllerBase
{
    /// <summary>
    /// Sends a message to the server.
    /// </summary>
    /// <param name="msg">The message to send. Content is limited to 128 characters.</param>
    /// <returns>HTTP 200 OK if the message was successfully processed.</returns>
    /// <response code="200">Message was successfully processed.</response>
    /// <response code="400">Invalid message data.</response>
    /// <response code="500">An error occurred while processing the request.</response>
    [HttpPost("send-message")]
    [ProducesResponseType(typeof(void), 200)]
    [ProducesResponseType(typeof(void), 400)]
    [ProducesResponseType(typeof(void), 500)]
    public async Task<IActionResult> PostMessage([FromBody] Msg msg)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        logger.LogInformation("Received a new message to save: {Content}", msg.Content);

        await messageService.SaveMessageAsync(msg); 
        await webSocketService.SendMessageToAllAsync(msg);

        logger.LogInformation("Message saved and sent to clients: {Content}", msg.Content);

        return Ok();
    }

    /// <summary>
    /// Retrieves a list of messages sent within a specific time range.
    /// </summary>
    /// <param name="startTime">The start time of the range.</param>
    /// <param name="endTime">The end time of the range.</param>
    /// <returns>A list of messages within the specified time range.</returns>
    /// <response code="200">Successfully retrieved the messages.</response>
    /// <response code="400">Invalid date range provided.</response>
    /// <response code="500">An error occurred while processing the request.</response>
    [HttpGet("get-messages")]
    [ProducesResponseType(typeof(IEnumerable<Msg>), 200)]
    [ProducesResponseType(typeof(void), 400)]
    [ProducesResponseType(typeof(void), 500)]
    public async Task<IActionResult> GetMessages([FromQuery] DateTime startTime, [FromQuery] DateTime endTime)
    {
        logger.LogInformation("Fetching messages between {StartTime} and {EndTime}", startTime, endTime);

        var messages = await messageService.GetMessagesAsync(startTime, endTime);

        logger.LogInformation("Fetched {Count} messages", messages.LongCount());

        return Ok(messages);
    }
}
