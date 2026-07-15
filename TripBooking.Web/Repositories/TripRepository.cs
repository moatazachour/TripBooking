using Microsoft.Data.SqlClient;
using System.Data;
using TripBooking.Web.Data;
using TripBooking.Web.Models.Entities;
using TripBooking.Web.Repositories.Interfaces;

namespace TripBooking.Web.Repositories;

public class TripRepository : ITripRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public TripRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<Trip>> GetAllAsync()
    {
        var trips = new List<Trip>();

        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("dbo.Trip_GetAll", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            trips.Add(MapTrip(reader, includeTravelerName: true));
        }

        return trips;
    }

    public async Task<List<Trip>> GetByTravelerIdAsync(int travelerId)
    {
        var trips = new List<Trip>();

        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("dbo.Trip_GetByTravelerId", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@TravelerId", travelerId);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            trips.Add(MapTrip(reader, includeTravelerName: false));
        }

        return trips;
    }

    public async Task<Trip?> GetByIdAsync(int tripId)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("dbo.Trip_GetById", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@TripId", tripId);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return MapTrip(reader, includeTravelerName: false);
        }

        return null;
    }

    public async Task<int> InsertAsync(Trip trip)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("dbo.Trip_Insert", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@TravelerId", trip.TravelerId);
        command.Parameters.AddWithValue("@Destination", trip.Destination);
        command.Parameters.AddWithValue("@StartDate", trip.StartDate.ToDateTime(TimeOnly.MinValue));
        command.Parameters.AddWithValue("@EndDate", trip.EndDate.ToDateTime(TimeOnly.MinValue));
        command.Parameters.AddWithValue("@Budget", trip.Budget);
        command.Parameters.AddWithValue("@Status", trip.Status);
        command.Parameters.AddWithValue("@DocumentPath", (object?)trip.DocumentPath ?? DBNull.Value);

        var outputIdParam = new SqlParameter("@NewTripId", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(outputIdParam);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();

        return (int)outputIdParam.Value;
    }

    public async Task UpdateAsync(Trip trip)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("dbo.Trip_Update", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@TripId", trip.TripId);
        command.Parameters.AddWithValue("@Destination", trip.Destination);
        command.Parameters.AddWithValue("@StartDate", trip.StartDate.ToDateTime(TimeOnly.MinValue));
        command.Parameters.AddWithValue("@EndDate", trip.EndDate.ToDateTime(TimeOnly.MinValue));
        command.Parameters.AddWithValue("@Budget", trip.Budget);
        command.Parameters.AddWithValue("@Status", trip.Status);
        command.Parameters.AddWithValue("@DocumentPath", (object?)trip.DocumentPath ?? DBNull.Value);
        command.Parameters.AddWithValue("@RowVersion", trip.RowVersion);

        await connection.OpenAsync();

        try
        {
            await command.ExecuteNonQueryAsync();
        }
        catch (SqlException ex) when (ex.Message.Contains("Concurrency conflict"))
        {
            // Translate the stored procedure's RAISERROR into a typed .NET
            // exception our Service/Controller layers can actually catch and
            // react to sensibly, instead of every caller having to string-match
            // a raw SQL error message. Full explanation in section 6.6.
            throw new DbConcurrencyException(
                "This trip was modified by someone else after you loaded it. Please reload and try again.");
        }
    }

    public async Task DeleteAsync(int tripId)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("dbo.Trip_Delete", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@TripId", tripId);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    private static Trip MapTrip(SqlDataReader reader, bool includeTravelerName)
    {
        var trip = new Trip
        {
            TripId = reader.GetInt32(reader.GetOrdinal("TripId")),
            TravelerId = reader.GetInt32(reader.GetOrdinal("TravelerId")),
            Destination = reader.GetString(reader.GetOrdinal("Destination")),
            StartDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("StartDate"))),
            EndDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("EndDate"))),
            Budget = reader.GetDecimal(reader.GetOrdinal("Budget")),
            Status = reader.GetString(reader.GetOrdinal("Status")),
            CreatedAtUtc = reader.GetDateTime(reader.GetOrdinal("CreatedAtUtc")),
            RowVersion = (byte[])reader["RowVersion"]
        };

        if (includeTravelerName)
        {
            trip.TravelerFirstName = reader.GetString(reader.GetOrdinal("FirstName"));
            trip.TravelerLastName = reader.GetString(reader.GetOrdinal("LastName"));
        }

        return trip;
    }
}