using ChatService.Http.Contracts;

namespace ChatService.Http.Converters.Implementation;

public static class MessageRequestToMessageDtoConverter
{
    public static MessageDto Convert(MessageRequest source)
        => new MessageDto
        {
            Id = source.Id,
            Content = source.Content,
            Date = DateTime.UtcNow
        };
}