using Dapper;
using Npgsql;
using NotificationService.DataAccess.Interfaces;
using NotificationService.Models;
using Microsoft.Extensions.Configuration;

namespace NotificationService.DataAccess;

public class UserRepository : IUserRepository
{
    private readonly string? _connectionString;

    public UserRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        return await conn.QueryFirstOrDefaultAsync<User>(
            "SELECT * FROM users WHERE email = @Email",
            new { Email = email });
    }

    public async Task CreateAsync(User user)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.ExecuteAsync(@"
            INSERT INTO users (name, email, password, created_at)
            VALUES (@Name, @Email, @Password, @CreatedAt)
        ", user);
    }
}
