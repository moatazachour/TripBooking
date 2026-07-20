using Microsoft.Data.SqlClient;
using System.Data;
using TripBooking.Web.Data;
using TripBooking.Web.Models.ViewModels;
using TripBooking.Web.Repositories.Interfaces;

namespace TripBooking.Web.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ReportRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<ActivityRevenueReportRow>> GetRevenueByActivityAsync()
    {
        var rows = new List<ActivityRevenueReportRow>();

        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("dbo.Report_RevenueByActivity", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            rows.Add(new ActivityRevenueReportRow
            {
                ActivityId = reader.GetInt32(reader.GetOrdinal("ActivityId")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                // TotalRevenue and TotalUnitsBooked come from SUM(...) over a
                // LEFT JOIN — an Activity with zero bookings produces SQL NULL
                // for both (SUM of nothing is NULL, not 0), so IsDBNull checks
                // are genuinely necessary here, not defensive paranoia.
                TotalRevenue = reader.IsDBNull(reader.GetOrdinal("TotalRevenue"))
                    ? 0m
                    : reader.GetDecimal(reader.GetOrdinal("TotalRevenue")),
                TotalUnitsBooked = reader.IsDBNull(reader.GetOrdinal("TotalUnitsBooked"))
                    ? 0
                    : reader.GetInt32(reader.GetOrdinal("TotalUnitsBooked")),
                TripCount = reader.GetInt32(reader.GetOrdinal("TripCount"))
            });
        }

        return rows;
    }

    public async Task<List<DestinationReportRow>> GetTripsByDestinationAsync(int? year)
    {
        var rows = new List<DestinationReportRow>();

        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("dbo.Report_TripsByDestination", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@Year", (object?)year ?? DBNull.Value);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            rows.Add(new DestinationReportRow
            {
                Destination = reader.GetString(reader.GetOrdinal("Destination")),
                TripCount = reader.GetInt32(reader.GetOrdinal("TripCount")),
                TotalBudget = reader.GetDecimal(reader.GetOrdinal("TotalBudget")),
                AverageBudget = reader.GetDecimal(reader.GetOrdinal("AverageBudget"))
            });
        }

        return rows;
    }
}