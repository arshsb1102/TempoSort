using Dapper;
using Npgsql;
using NotificationService.DataAccess.Interfaces;
using NotificationService.Models;
using Microsoft.Extensions.Configuration;
using NotificationService.Models.DBObjects;
using System.Data;

namespace NotificationService.DataAccess;

public class UserRepository : IUserRepository
{
    private readonly string? _connectionString;
    private readonly IConnectionFactory _connectionFactory;

    public UserRepository(IConfiguration configuration, IConnectionFactory connectionFactory)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        _connectionFactory = connectionFactory;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        return await conn.QueryFirstOrDefaultAsync<User>(
            "SELECT * FROM users WHERE email = @Email",
            new { Email = email });
    }
    public async Task CreateUserAsync(User user)
    {
        const string sql = @"
        INSERT INTO Users (UserId, Name, Email, Password, CreatedOn, IsEmailVerified, IsEmailDead)
        VALUES (@UserId, @Name, @Email, @Password, @CreatedOn, @IsEmailVerified, @IsEmailDead);";

        using var conn = _connectionFactory.GetOpenConnection();
        await conn.ExecuteAsync(sql, user);
    }
    public async Task MarkEmailVerified(Guid userId)
    {
        const string sql = @"
        UPDATE users
        SET isEmailVerified = true,
            verifiedon = NOW()
        WHERE userid = @userId";

        using var conn = _connectionFactory.GetOpenConnection();
        await conn.ExecuteAsync(sql, new { userId });
    }
    public async Task UpdateLastVerificationSentOnAsync(Guid userId)
    {
        const string query = @"
        UPDATE users 
        SET lastverificationsenton = @Timestamp 
        WHERE userid = @UserId;
    ";

        using var conn = _connectionFactory.GetOpenConnection();
        await conn.ExecuteAsync(query, new { UserId = userId, Timestamp = DateTime.UtcNow });
    }
    public async Task UpdateWelcomeOnAsync(Guid userId)
    {
        const string query = @"
        UPDATE users 
        SET iswelcomedone = true 
        WHERE userid = @UserId;";

        using var conn = _connectionFactory.GetOpenConnection();
        await conn.ExecuteAsync(query, new { UserId = userId, Timestamp = DateTime.UtcNow });
    }

}
