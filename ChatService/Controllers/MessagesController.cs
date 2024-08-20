using ChatService.Contracts.Http;
using ChatService.Contracts.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ChatService.Hubs;
using ChatService.Services.Message;

namespace ChatService.Controllers;

[ApiController]
[Route("api/v1/message")]
public class MessagesController(
    IMessageService messageService,
    IHubContext<ChatHub> hubContext,
    ILogger<MessagesController> logger)
    : ControllerBase
{
    /// <summary>
    /// Отправляет сообщение на сервер.
    /// </summary>
    /// <param name="messageRequest">Сообщение для отправки. ID и Content обязательны.</param>
    /// <param name="cancellationToken">Токен отмены для запроса.</param>
    /// <returns>HTTP 200 OK, если сообщение было успешно обработано.</returns>
    /// <response code="200">Сообщение успешно обработано.</response>
    /// <response code="400">Некорректные данные сообщения.</response>
    /// <response code="500">Произошла ошибка при обработке запроса.</response>
    [HttpPost("send-message")]
    [ProducesResponseType(typeof(void), 200)]
    [ProducesResponseType(typeof(void), 400)]
    [ProducesResponseType(typeof(void), 500)]
    public async Task<IActionResult> PostMessage([FromBody] MessageRequest messageRequest, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        logger.LogInformation($"{nameof(MessagesController)}.{nameof(PostMessage)}: Received a new message to save: {new { messageRequest.Id, messageRequest.Content }}");

        try
        {
            var messageDto = new MessageDto
            {
                Id = messageRequest.Id,
                Content = messageRequest.Content,
                Date = DateTime.UtcNow
            };

            var savedMessage = await messageService.SaveMessageAsync(messageDto, cancellationToken);

            await hubContext.Clients.All.SendAsync("ReceiveMessage", new MessageSignalRDto
            {
                Id = savedMessage.Id,
                Content = savedMessage.Content,
                Date = savedMessage.Date
            }, cancellationToken);

            logger.LogInformation($"{nameof(MessagesController)}.{nameof(PostMessage)}: Message saved and sent to clients: {new { savedMessage.Id, savedMessage.Content, savedMessage.Date }}");

            return Ok();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, $"{nameof(MessagesController)}.{nameof(PostMessage)}: Error occurred while saving or sending the message.");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    /// <summary>
    /// Получает список сообщений, отправленных в определенном диапазоне времени.
    /// </summary>
    /// <param name="startTime">Время начала диапазона.</param>
    /// <param name="endTime">Время конца диапазона.</param>
    /// <param name="cancellationToken">Токен отмены для запроса.</param>
    /// <returns>Список сообщений в указанном диапазоне времени.</returns>
    /// <response code="200">Сообщения успешно получены.</response>
    /// <response code="400">Некорректный диапазон дат.</response>
    /// <response code="500">Произошла ошибка при обработке запроса.</response>
    [HttpGet("get-messages")]
    [ProducesResponseType(typeof(IEnumerable<MessageDto>), 200)]
    [ProducesResponseType(typeof(void), 400)]
    [ProducesResponseType(typeof(void), 500)]
    public async Task<IActionResult> GetMessages([FromQuery] DateTime startTime, [FromQuery] DateTime endTime, CancellationToken cancellationToken)
    {
        if (startTime >= endTime)
        {
            return BadRequest("Invalid date range.");
        }

        logger.LogInformation($"{nameof(MessagesController)}.{nameof(GetMessages)}: Fetching messages between {startTime} and {endTime}");

        try
        {
            var messages = await messageService.GetMessagesAsync(startTime, endTime, cancellationToken);

            logger.LogInformation($"{nameof(MessagesController)}.{nameof(GetMessages)}: Fetched {messages.Count()} messages");

            return Ok(messages);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, $"{nameof(MessagesController)}.{nameof(GetMessages)}: Error occurred while retrieving messages.");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}
