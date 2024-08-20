namespace ChatService.Data.DataConnect
{
    public interface IDataConnect
    {
        Task<int> ExecuteScalarAsync<T>(string query, Dictionary<string, object> parameters, CancellationToken cancellationToken);
        Task<IEnumerable<Dictionary<string, object>>> ExecuteQueryAsync(string query, Dictionary<string, object> parameters, CancellationToken cancellationToken);
    }
}