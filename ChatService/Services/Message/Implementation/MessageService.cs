using ChatService.Data.Converters;
using ChatService.Data.MessageRepository;
using ChatService.Http.Contracts;

namespace ChatService.Services.Message.Implementation;

public class MessageService(IMessageRepository messageRepository) : IMessageService
{
    public async Task SaveMessageAsync(MessageDto messageDto, CancellationToken cancellationToken)
    {
        var messageDataDto = MessageConverter.ToMessageDataDto(messageDto);
        await messageRepository.SaveMessageAsync(messageDataDto, cancellationToken);
    }

    public async Task<IEnumerable<MessageDto>> GetMessagesAsync(DateTime startTime, DateTime endTime, CancellationToken cancellationToken)
    {
        var dataMessages = await messageRepository.GetMessagesAsync(startTime, endTime, cancellationToken);
        var messageDtos = dataMessages.Select(MessageConverter.ToMessageDto).ToList();
        return messageDtos;
    }
}