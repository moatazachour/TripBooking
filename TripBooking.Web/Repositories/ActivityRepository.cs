using Microsoft.Data.SqlClient;
using System.Data;
using TripBooking.Web.Data;
using TripBooking.Web.Models.Entities;
using TripBooking.Web.Repositories.Interfaces;

namespace TripBooking.Web.Repositories;

public class ActivityRepository : IActivityRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ActivityRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<Activity>> GetAllAsync()
    {
        var activities = new List<Activity>();

        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("dbo.Activity_GetAll", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            activities.Add(MapActivity(reader));
        }

        return activities;
    }

    public async Task<Activity?> GetByIdAsync(int activityId)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("dbo.Activity_GetById", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@ActivityId", activityId);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return MapActivity(reader);
        }

        return null;
    }

    public async Task<int> InsertAsync(Activity activity)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("dbo.Activity_Insert", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@Name", activity.Name);
        command.Parameters.AddWithValue("@Description", (object?)activity.Description ?? DBNull.Value);
        command.Parameters.AddWithValue("@UnitPrice", activity.UnitPrice);
        command.Parameters.AddWithValue("@IsActive", activity.IsActive);

        // OUTPUT parameters need Direction set explicitly — ADO.NET has no way
        // to know a parameter is an OUTPUT parameter just from its name.
        var outputIdParam = new SqlParameter("@NewActivityId", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(outputIdParam);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();  // no rows returned, just the OUTPUT param

        return (int)outputIdParam.Value;
    }

    public async Task UpdateAsync(Activity activity)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("dbo.Activity_Update", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@ActivityId", activity.ActivityId);
        command.Parameters.AddWithValue("@Name", activity.Name);
        command.Parameters.AddWithValue("@Description", (object?)activity.Description ?? DBNull.Value);
        command.Parameters.AddWithValue("@UnitPrice", activity.UnitPrice);
        command.Parameters.AddWithValue("@IsActive", activity.IsActive);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(int activityId)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("dbo.Activity_Delete", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@ActivityId", activityId);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    // Centralizing the row->object mapping in one private method avoids
    // repeating the same column-index/ordinal lookups in every method,
    // and it's the one place to fix if a column is ever added or renamed.
    private static Activity MapActivity(SqlDataReader reader)
    {
        return new Activity
        {
            ActivityId = reader.GetInt32(reader.GetOrdinal("ActivityId")),
            Name = reader.GetString(reader.GetOrdinal("Name")),
            Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                ? null
                : reader.GetString(reader.GetOrdinal("Description")),
            UnitPrice = reader.GetDecimal(reader.GetOrdinal("UnitPrice")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
        };
    }
}
