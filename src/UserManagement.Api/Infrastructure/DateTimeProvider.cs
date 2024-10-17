namespace UserManagement.Api.Infrastructure;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
