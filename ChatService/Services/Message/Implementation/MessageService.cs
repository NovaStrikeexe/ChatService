using ChatService.Contracts.Http;
using ChatService.Data;
using ChatService.Data.MessageRepository;
using ChatService.Services.Message;

namespace ChatService.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;

        public MessageService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<MessageDto> SaveMessageAsync(MessageDto messageDto, CancellationToken cancellationToken)
        {
            return await _messageRepository.SaveMessageAsync(messageDto, cancellationToken);
        }

        public async Task<IEnumerable<MessageDto>> GetMessagesAsync(DateTime startTime, DateTime endTime, CancellationToken cancellationToken)
        {
            return await _messageRepository.GetMessagesAsync(startTime, endTime, cancellationToken);
        }
    }
}