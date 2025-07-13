using Dapper;
using NotificationService.DataAccess.Interfaces;
using NotificationService.Models.DBObjects;
using System.Data;

namespace NotificationService.DataAccess.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly IDbConnection _connection;

    public TaskRepository(IConnectionFactory connectionFactory)
    {
        _connection = connectionFactory.GetOpenConnection();
    }

    public async Task CreateAsync(UserTask task)
    {
        const string sql = @"
            INSERT INTO user_tasks (taskid, userid, title, description, dueat, iscompleted, createdon, updatedon)
            VALUES (@TaskId, @UserId, @Title, @Description, @dueat, @IsCompleted, @CreatedOn, @UpdatedOn)";
        await _connection.ExecuteAsync(sql, task);
    }

    public async Task<IEnumerable<UserTask>> GetByUserIdAsync(Guid userId)
    {
        const string sql = "SELECT * FROM user_tasks WHERE userid = @UserId ORDER BY dueat";
        return await _connection.QueryAsync<UserTask>(sql, new { UserId = userId });
    }

    public async Task<UserTask?> GetByIdAsync(Guid taskId)
    {
        const string sql = "SELECT * FROM user_tasks WHERE taskid = @TaskId";
        return await _connection.QueryFirstOrDefaultAsync<UserTask>(sql, new { TaskId = taskId });
    }

    public async Task DeleteAsync(Guid taskId, Guid userId)
    {
        const string sql = "DELETE FROM user_tasks WHERE taskid = @TaskId AND userid = @UserId";
        await _connection.ExecuteAsync(sql, new { TaskId = taskId, UserId = userId });
    }

    public async Task UpdateAsync(UserTask task)
    {
        const string sql = @"
            UPDATE user_tasks
            SET title = @Title,
                description = @Description,
                dueat = @dueat,
                iscompleted = @IsCompleted,
                updatedon = @UpdatedOn
            WHERE taskid = @TaskId AND userid = @UserId";
        await _connection.ExecuteAsync(sql, task);
    }
}
