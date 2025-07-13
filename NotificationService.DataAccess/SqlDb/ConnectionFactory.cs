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

    public IDbConnection GetOpenConnection()
    {
        var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        return conn;
    }
}
