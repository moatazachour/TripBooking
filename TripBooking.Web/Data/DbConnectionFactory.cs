using Microsoft.Data.SqlClient;

namespace TripBooking.Web.Data;

public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("TripBookingDB")
            ?? throw new InvalidOperationException("Connection string 'TripBookingDB' not found.");
    }

    public SqlConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
