using Microsoft.Data.SqlClient;
using System.Data;
using TripBooking.Web.Data;
using TripBooking.Web.Models.Entities;
using TripBooking.Web.Repositories.Interfaces;

namespace TripBooking.Web.Repositories;

public class TripActivityRepository : ITripActivityRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public TripActivityRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<TripActivity>> GetByTripIdAsync(int tripId)
    {
        var tripActivities = new List<TripActivity>();

        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("dbo.TripActivity_GetByTripId", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@TripId", tripId);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            tripActivities.Add(new TripActivity
            {
                TripActivityId = reader.GetInt32(reader.GetOrdinal("TripActivityId")),
                TripId = reader.GetInt32(reader.GetOrdinal("TripId")),
                ActivityId = reader.GetInt32(reader.GetOrdinal("ActivityId")),
                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                PriceAtBooking = reader.GetDecimal(reader.GetOrdinal("PriceAtBooking")),
                RowVersion = (byte[])reader["RowVersion"],
                ActivityName = reader.GetString(reader.GetOrdinal("ActivityName")),
                CurrentUnitPrice = reader.GetDecimal(reader.GetOrdinal("CurrentUnitPrice"))
            });
        }

        return tripActivities;
    }

    public async Task<int> InsertAsync(int tripId, int activityId, int quantity)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("dbo.TripActivity_Insert", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@TripId", tripId);
        command.Parameters.AddWithValue("@ActivityId", activityId);
        command.Parameters.AddWithValue("@Quantity", quantity);

        var outputIdParam = new SqlParameter("@NewTripActivityId", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(outputIdParam);

        await connection.OpenAsync();

        try
        {
            await command.ExecuteNonQueryAsync();
        }
        catch (SqlException ex) when (ex.Message.Contains("not found or not active"))
        {
            throw new InvalidOperationException("This activity is no longer available for booking.");
        }

        return (int)outputIdParam.Value;
    }

    public async Task UpdateQuantityAsync(int tripActivityId, int quantity, byte[] rowVersion)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("dbo.TripActivity_UpdateQuantity", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@TripActivityId", tripActivityId);
        command.Parameters.AddWithValue("@Quantity", quantity);
        command.Parameters.AddWithValue("@RowVersion", rowVersion);

        await connection.OpenAsync();

        try
        {
            await command.ExecuteNonQueryAsync();
        }
        catch (SqlException ex) when (ex.Message.Contains("Concurrency conflict"))
        {
            throw new DbConcurrencyException(
                "This booking was modified by someone else after you loaded it. Please reload and try again.");
        }
    }

    public async Task DeleteAsync(int tripActivityId)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("dbo.TripActivity_Delete", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@TripActivityId", tripActivityId);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }
}