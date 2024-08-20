using ChatService.Data.Contracts;
using ChatService.Data.DataConnect;

namespace ChatService.Data.MessageRepository.Implementation
{
    public class MessageRepository(
        ILogger<MessageRepository> logger,
        IDataConnect dataConnect)
        : IMessageRepository
    {
        public async Task SaveMessageAsync(MessageDataDto messageDataDto, CancellationToken cancellationToken)
        {
            const string query = @"
                INSERT INTO messages (id, content, date) 
                VALUES (@Id, @Content, @Date) 
                ON CONFLICT (id) 
                DO UPDATE SET content = EXCLUDED.content, date = EXCLUDED.date
                RETURNING id, content, date;";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", messageDataDto.Id },
                { "@Content", messageDataDto.Content },
                { "@Date", messageDataDto.Date }
            };

            try
            {
                await dataConnect.ExecuteQueryAsync(query, parameters, cancellationToken);
                logger.LogWarning("No rows returned from SaveMessageAsync.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while saving the message.");
                throw;
            }
        }

        public async Task<IEnumerable<MessageDataDto>> GetMessagesAsync(DateTime startTime, DateTime endTime, CancellationToken cancellationToken)
        {
            const string query = @"
                SELECT id, content, date 
                FROM messages 
                WHERE date >= @StartTime AND date <= @EndTime;";

            var parameters = new Dictionary<string, object>
            {
                { "@StartTime", startTime },
                { "@EndTime", endTime }
            };

            try
            {
                var result = await dataConnect.ExecuteQueryAsync(query, parameters, cancellationToken);

                return result.Select(row => new MessageDataDto { Id = Convert.ToInt32(row["id"]), Content = Convert.ToString(row["content"]), Date = Convert.ToDateTime(row["date"]) }).ToList();
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "An error occurred while retrieving messages.");
                throw;
            }
        }
    }
}
