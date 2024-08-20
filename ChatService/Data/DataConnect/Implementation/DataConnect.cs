using Npgsql;

namespace ChatService.Data.DataConnect.Implementation
{
    public class DataConnect(string connectionString) : IDataConnect
    {
        public async Task<int> ExecuteScalarAsync<T>(string query, Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new NpgsqlCommand(query, connection);
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value);
            }

            return (int)await command.ExecuteScalarAsync(cancellationToken);
        }

        public async Task<IEnumerable<Dictionary<string, object>>> ExecuteQueryAsync(string query, Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            var results = new List<Dictionary<string, object>>();

            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new NpgsqlCommand(query, connection);
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value);
            }

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                var row = new Dictionary<string, object>();
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    row[reader.GetName(i)] = reader.GetValue(i);
                }
                results.Add(row);
            }

            return results;
        }
    }
}