using ChatService.Data.Contracts;
using ChatService.Http.Contracts;

namespace ChatService.Data.Converters;

public static class MessageConverter
{
    public static MessageDto ToMessageDto(MessageDataDto source)
    {
        return new MessageDto
        {
            Id = source.Id,
            Content = source.Content,
            Date = source.Date
        };
    }

    public static MessageDataDto ToMessageDataDto(MessageDto source)
    {
        return new MessageDataDto
        {
            Id = source.Id,
            Content = source.Content,
            Date = source.Date
        };
    }
}