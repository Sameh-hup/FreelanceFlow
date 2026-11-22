namespace App.BLL.Services.Clock;

public class SystemClock : IClock
{
    private static readonly TimeZoneInfo CairoTimeZone;

    static SystemClock()
    {
        try
        {
            CairoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
        }
        catch (TimeZoneNotFoundException)
        {
            CairoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo");
        }
    }

    public DateTime UtcNow => DateTime.UtcNow;

    public DateTime CairoNow => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, CairoTimeZone);

    public DateOnly TodayInCairo => DateOnly.FromDateTime(CairoNow);
}
