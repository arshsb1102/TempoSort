using System.Data;

public interface IConnectionFactory
{
    IDbConnection GetOpenConnection();
}
