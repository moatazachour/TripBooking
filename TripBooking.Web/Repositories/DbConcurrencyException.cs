namespace TripBooking.Web.Repositories;

// A small, purpose-built exception type. Deriving from Exception (not
// InvalidOperationException or some built-in type) means callers can
// catch specifically "a concurrency conflict happened" without accidentally
// also catching unrelated bugs that happen to throw InvalidOperationException.
public class DbConcurrencyException : Exception
{
    public DbConcurrencyException(string message) : base(message) { }
}