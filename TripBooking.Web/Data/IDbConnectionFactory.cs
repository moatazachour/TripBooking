using Microsoft.Data.SqlClient;

namespace TripBooking.Web.Data;

public interface IDbConnectionFactory
{
    SqlConnection CreateConnection();
}
