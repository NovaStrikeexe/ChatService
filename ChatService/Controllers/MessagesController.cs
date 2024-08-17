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
    /// <param name="Content">The message to send. Content is limited to 128 characters.</param>
    /// <returns>HTTP 200 OK if the message was successfully processed.</returns>
    /// <response code="200">Message was successfully processed.</response>
    /// <response code="400">Invalid message data.</response>
    /// <response code="500">An error occurred while processing the request.</response>
    [HttpPost("send-message")]
    [ProducesResponseType(typeof(void), 200)]
    [ProducesResponseType(typeof(void), 400)]
    [ProducesResponseType(typeof(void), 500)]
    public async Task<IActionResult> PostMessage([FromBody] Msg message)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        logger.LogInformation($"{nameof(MessagesController)}{nameof(PostMessage)}: Received a new message to save: {new{message.Id, message.Content}}");

        var savedMessage = await messageService.SaveMessageAsync(message); 
        await webSocketService.SendMessageToAllAsync(savedMessage);

        logger.LogInformation($"{nameof(MessagesController)}{nameof(PostMessage)}: Message saved and sent to clients: {new {savedMessage.Id, savedMessage.Content, savedMessage.Date}}");

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
    [ProducesResponseType(typeof(IEnumerable<MsgDto>), 200)]
    [ProducesResponseType(typeof(void), 400)]
    [ProducesResponseType(typeof(void), 500)]
    public async Task<IActionResult> GetMessages([FromQuery] DateTime startTime, [FromQuery] DateTime endTime)
    {
        logger.LogInformation($"{nameof(MessagesController)}{nameof(GetMessages)}: Fetching messages between {startTime} and {endTime}");

        var messages = await messageService.GetMessagesAsync(startTime, endTime);

        logger.LogInformation($"{nameof(MessagesController)}{nameof(GetMessages)}: Fetched { (int)messages.LongCount()} messages");

        return Ok(messages);
    }
}
