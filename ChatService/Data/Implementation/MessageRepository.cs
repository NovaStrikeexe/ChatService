using ChatService.Configuration.Models;
using ChatService.Models;
using Microsoft.Extensions.Options;
using Npgsql;

namespace ChatService.Data.Implementation;

public class MessageRepository(ILogger<MessageRepository> logger, IOptions<DbSettings> dbSettings)
    : IMessageRepository
{
    private readonly string _connectionString = dbSettings.Value.DefaultConnection 
                                                ?? throw new ArgumentNullException(nameof(dbSettings.Value.DefaultConnection), "Connection string cannot be null.");

    public async Task<MsgDto> SaveMessageAsync(Msg msg)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var date = DateTime.UtcNow;

            var query = @" INSERT INTO messages (id, content, date) VALUES (@Id, @Content, @Date) ON CONFLICT (id) 
                DO UPDATE SET content = EXCLUDED.content, date = EXCLUDED.date;";
            await using var command = new NpgsqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", msg.Id);
            command.Parameters.AddWithValue("@Content", msg.Content);
            command.Parameters.AddWithValue("@Date", date);

            await command.ExecuteNonQueryAsync();

            return new MsgDto
            {
                Id = msg.Id,
                Content = msg.Content,
                Date = date
            };
        }
        catch (Exception exception)
        {
            logger.LogError(exception, $"{nameof(MessageRepository)}.{nameof(SaveMessageAsync)}");
            throw;
        }
    }

    public async Task<IEnumerable<MsgDto>> GetMessagesAsync(DateTime startTime, DateTime endTime)
    {
        var messages = new List<MsgDto>();

        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT id, content, date FROM messages WHERE date BETWEEN @startTime AND @endTime ORDER BY date";
            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@startTime", startTime);
            command.Parameters.AddWithValue("@endTime", endTime);

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var message = new MsgDto
                {
                    Id = reader.GetInt32(0),
                    Content = reader.GetString(1),
                    Date = reader.GetDateTime(2)
                };
                messages.Add(message);
            }
        }
        catch (Exception exception)
        {
            logger.LogError(exception, $"{nameof(MessageRepository)}.{nameof(GetMessagesAsync)} failed for range: {startTime} - {endTime}");
            throw;
        }

        return messages;
    }
}