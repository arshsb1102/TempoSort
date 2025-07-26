using System.Data;
using Npgsql;

public interface IConnectionFactory
{
    Task<NpgsqlConnection> GetOpenConnectionAsync();
}
