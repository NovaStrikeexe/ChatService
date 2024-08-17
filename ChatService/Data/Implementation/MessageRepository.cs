using ChatService.Models;
using Npgsql;

namespace ChatService.Data.Implementation;


public class MessageRepository(ILogger<MessageRepository> logger, IConfiguration configuration)
    : IMessageRepository
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection") 
                                                ?? throw new ArgumentNullException(nameof(_connectionString), "Connection string cannot be null.");

    public async Task SaveMessageAsync(Msg msg)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "INSERT INTO Messages (Content, CreatedAt) VALUES (@content, @createdAt)";
            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@content", msg.Content);
            command.Parameters.AddWithValue("@createdAt", msg.Date);

            await command.ExecuteNonQueryAsync();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, $"{nameof(MessageRepository)}.{nameof(SaveMessageAsync)}");
            throw;
        }
    }

    public async Task<IEnumerable<Msg>> GetMessagesAsync(DateTime startTime, DateTime endTime)
    {
        var messages = new List<Msg>();

        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT Id, Content, CreatedAt FROM Messages WHERE CreatedAt BETWEEN @startTime AND @endTime ORDER BY CreatedAt";
            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@startTime", startTime);
            command.Parameters.AddWithValue("@endTime", endTime);

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var message = new Msg
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