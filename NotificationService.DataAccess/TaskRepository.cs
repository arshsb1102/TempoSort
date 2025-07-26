using Dapper;
using NotificationService.DataAccess.Interfaces;
using NotificationService.Models.DBObjects;
using System.Data;

namespace NotificationService.DataAccess.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public TaskRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task CreateAsync(UserTask task)
    {
        using var connection = _connectionFactory.GetOpenConnection();
        const string sql = @"
            INSERT INTO tasks (taskid, userid, title, description, dueat, iscompleted, createdon, updatedon)
            VALUES (@TaskId, @UserId, @Title, @Description, @dueat, @IsCompleted, @CreatedOn, @UpdatedOn)";
        await connection.ExecuteAsync(sql, task);
    }

    public async Task<IEnumerable<UserTask>> GetByUserIdAsync(Guid userId)
    {
        using var connection = _connectionFactory.GetOpenConnection();
        const string sql = "SELECT * FROM tasks WHERE userid = @UserId ORDER BY dueat";
        return await connection.QueryAsync<UserTask>(sql, new { UserId = userId });
    }

    public async Task<UserTask?> GetByIdAsync(Guid taskId)
    {
        using var connection = _connectionFactory.GetOpenConnection();
        const string sql = "SELECT * FROM tasks WHERE taskid = @TaskId";
        return await connection.QueryFirstOrDefaultAsync<UserTask>(sql, new { TaskId = taskId });
    }

    public async Task DeleteAsync(Guid taskId, Guid userId)
    {
        using var connection = _connectionFactory.GetOpenConnection();
        const string sql = "DELETE FROM tasks WHERE taskid = @TaskId AND userid = @UserId";
        await connection.ExecuteAsync(sql, new { TaskId = taskId, UserId = userId });
    }

    public async Task UpdateAsync(UserTask task)
    {
        using var connection = _connectionFactory.GetOpenConnection();
        const string sql = @"
            UPDATE tasks
            SET title = @Title,
                description = @Description,
                dueat = @dueat,
                iscompleted = @IsCompleted,
                updatedon = @UpdatedOn
            WHERE taskid = @TaskId AND userid = @UserId";
        await connection.ExecuteAsync(sql, task);
    }
}
