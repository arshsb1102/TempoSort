using NotificationService.Models;
using NotificationService.DataAccess.Interfaces;
using Microsoft.Extensions.Configuration;
using Dapper;
using Npgsql;
public class NotificationRepository : INotificationRepository
{
    private readonly string _conn;

    public NotificationRepository(IConfiguration config)
    {
        _conn = config.GetConnectionString("DefaultConnection")!;
    }

    public async Task CreateAsync(Notification notification)
    {
        using var conn = new NpgsqlConnection(_conn);
        var sql = "INSERT INTO notifications (user_id, message, link, type) VALUES (@UserId, @Message, @Link, @Type)";
        await conn.ExecuteAsync(sql, notification);
    }

    public async Task<IEnumerable<Notification>> GetByUserIdAsync(int userId)
    {
        using var conn = new NpgsqlConnection(_conn);
        var sql = @"SELECT
            id, 
            user_id AS UserId, 
            message, 
            link, 
            type, 
            is_read AS IsRead, 
            created_at AS CreatedAt
        FROM notifications
        WHERE user_id = @userId
        ORDER BY created_at DESC";
        return await conn.QueryAsync<Notification>(sql, new { userId });
    }

    public async Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(int userId)
    {
        using var conn = new NpgsqlConnection(_conn);
        var sql = @"SELECT
            id, 
            user_id AS UserId, 
            message, 
            link, 
            type, 
            is_read AS IsRead, 
            created_at AS CreatedAt
        FROM notifications
        WHERE user_id = @userId AND is_read = false 
        ORDER BY created_at DESC";
        return await conn.QueryAsync<Notification>(sql, new { userId });
    }

    public async Task MarkAsReadAsync(int notificationId, int userId)
    {
        using var conn = new NpgsqlConnection(_conn);
        var sql = "UPDATE notifications SET is_read = true WHERE id = @notificationId AND user_id = @userId";
        await conn.ExecuteAsync(sql, new { notificationId, userId });
    }
}
