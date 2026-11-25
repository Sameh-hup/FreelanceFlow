namespace App.BLL.Services.Clock;

public class SystemClock : IClock
{
    private static readonly TimeZoneInfo CairoTimeZone = GetCairoTimeZone();

    public DateTime UtcNow => DateTime.UtcNow;

    public DateOnly UtcToday => DateOnly.FromDateTime(DateTime.UtcNow);

    public DateOnly CairoToday => DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, CairoTimeZone));

    private static TimeZoneInfo GetCairoTimeZone()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo");
        }
    }
}
