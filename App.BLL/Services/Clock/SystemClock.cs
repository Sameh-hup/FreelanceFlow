namespace App.BLL.Services.Clock;

public class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;

    public DateOnly UtcToday => DateOnly.FromDateTime(DateTime.UtcNow);
}
