using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

public class ConnectionFactory : IConnectionFactory
{
    private readonly string _connectionString;

    public ConnectionFactory(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")
                         ?? throw new InvalidOperationException("Connection string not found");
    }

    public async Task<NpgsqlConnection> GetOpenConnectionAsync()
    {
        var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync(); // ✅ Non-blocking connection
        return conn;
    }
}
