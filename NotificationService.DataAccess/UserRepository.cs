using Dapper;
using Npgsql;
using NotificationService.DataAccess.Interfaces;
using NotificationService.Models;
using Microsoft.Extensions.Configuration;
using NotificationService.Models.DBObjects;
using System.Data;
using NotificationService.Models.DTOs;

namespace NotificationService.DataAccess;

public class UserRepository : IUserRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public UserRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        await using var conn = await _connectionFactory.GetOpenConnectionAsync();
        return await conn.QueryFirstOrDefaultAsync<User>(
            "SELECT * FROM users WHERE email = @Email",
            new { Email = email });
    }
    public async Task CreateUserAsync(User user)
    {
        const string sql = @"
        INSERT INTO Users (UserId, Name, Email, Password, CreatedOn, IsEmailVerified, IsEmailDead)
        VALUES (@UserId, @Name, @Email, @Password, @CreatedOn, @IsEmailVerified, @IsEmailDead);";

        await using var conn = await _connectionFactory.GetOpenConnectionAsync();
        await conn.ExecuteAsync(sql, user);
    }
    public async Task MarkEmailVerified(Guid userId)
    {
        const string sql = @"
        UPDATE users
        SET isEmailVerified = true,
            verifiedon = NOW()
        WHERE userid = @userId";

        await using var conn = await _connectionFactory.GetOpenConnectionAsync();
        await conn.ExecuteAsync(sql, new { userId });
    }
    public async Task UpdateLastVerificationSentOnAsync(Guid userId)
    {
        const string query = @"
        UPDATE users 
        SET lastverificationsenton = @Timestamp 
        WHERE userid = @UserId;
    ";

        await using var conn = await _connectionFactory.GetOpenConnectionAsync();
        await conn.ExecuteAsync(query, new { UserId = userId, Timestamp = DateTime.UtcNow });
    }
    public async Task UpdateWelcomeOnAsync(Guid userId)
    {
        const string query = @"
        UPDATE users 
        SET iswelcomedone = true 
        WHERE userid = @UserId;";

        await using var conn = await _connectionFactory.GetOpenConnectionAsync();
        await conn.ExecuteAsync(query, new { UserId = userId, Timestamp = DateTime.UtcNow });
    }
    public async Task UpdateDigestSettingsAsync(Guid userId, UserPreferencesDto preferencesDto)
    {
        const string query = @"
        UPDATE users 
        SET 
            isdigestenabled = @IsDigestEnabled,
            digesttime = @DigestTime
        WHERE userid = @UserId;";

        await using var conn = await _connectionFactory.GetOpenConnectionAsync();
        await conn.ExecuteAsync(query, new { UserId = userId, preferencesDto.IsDigestEnabled, preferencesDto.DigestTime });
    }
    public async Task<IEnumerable<User>> GetUsersForDigestAsync(int hour, int minute)
    {
        const string query = @"
        SELECT * FROM users 
        WHERE isdigestenabled = true
          AND EXTRACT(HOUR FROM digesttime) = @Hour
          AND EXTRACT(MINUTE FROM digesttime) = @Minute;";

        await using var conn = await _connectionFactory.GetOpenConnectionAsync();
        return await conn.QueryAsync<User>(query, new { Hour = hour, Minute = minute });
    }
    public async Task<bool> DeleteUser(Guid userId)
    {
        try
        {
            const string query = @"
                Delete FROM users 
            WHERE userid = @UserId;";

            await using var conn = await _connectionFactory.GetOpenConnectionAsync();
            await conn.QueryAsync<User>(query, new { UserId = userId });
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SwitchUserDigest(Guid userId)
    {
        try
        {
            const string query = @" 
                UPDATE users 
                SET isdigestenabled = false
            ";

            await using var conn = await _connectionFactory.GetOpenConnectionAsync();
            await conn.QueryAsync<User>(query, new { UserId = userId });
            return true;
        }
        catch
        {
            return false;
        }
    }
}
