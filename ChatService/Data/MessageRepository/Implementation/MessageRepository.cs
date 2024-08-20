using ChatService.Configuration.Models;
using ChatService.Contracts.Http;
using ChatService.Data.DataConnect;
using Microsoft.Extensions.Options;

namespace ChatService.Data.MessageRepository.Implementation
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ILogger<MessageRepository> _logger;
        private readonly IDataConnect _dataConnect;
        private readonly string _connectionString;

        public MessageRepository(
            ILogger<MessageRepository> logger,
            IOptions<DbSettings> dbSettings,
            IDataConnect dataConnect)
        {
            _logger = logger;
            _dataConnect = dataConnect;
            _connectionString = dbSettings.Value.DefaultConnection;
        }

        public async Task<MessageDto> SaveMessageAsync(MessageDto message, CancellationToken cancellationToken)
        {
            const string query = @"
                INSERT INTO messages (id, content, date) 
                VALUES (@Id, @Content, @Date) 
                ON CONFLICT (id) 
                DO UPDATE SET content = EXCLUDED.content, date = EXCLUDED.date
                RETURNING id, content, date;";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", message.Id },
                { "@Content", message.Content },
                { "@Date", message.Date }
            };

            try
            {
                var result = await _dataConnect.ExecuteQueryAsync(query, parameters, cancellationToken);
                var resultRow = result.FirstOrDefault();

                if (resultRow != null)
                {
                    return new MessageDto
                    {
                        Id = Convert.ToInt32(resultRow["id"]),
                        Content = Convert.ToString(resultRow["content"]),
                        Date = Convert.ToDateTime(resultRow["date"])
                    };
                }

                _logger.LogWarning("No rows returned from SaveMessageAsync.");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the message.");
                throw;
            }
        }

        public async Task<IEnumerable<MessageDto>> GetMessagesAsync(DateTime startTime, DateTime endTime, CancellationToken cancellationToken)
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
                var result = await _dataConnect.ExecuteQueryAsync(query, parameters, cancellationToken);

                return result.Select(row => new MessageDto { Id = Convert.ToInt32(row["id"]), Content = Convert.ToString(row["content"]), Date = Convert.ToDateTime(row["date"]) }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving messages.");
                throw;
            }
        }
    }
}
