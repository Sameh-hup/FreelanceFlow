namespace App.BLL.Services.Clock;

public class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;

    public DateOnly UtcToday => DateOnly.FromDateTime(DateTime.UtcNow);

    public DateOnly CairoToday
    {
        get
        {
            TimeZoneInfo cairoZone;
            try
            {
                cairoZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
            }
            catch (TimeZoneNotFoundException)
            {
                cairoZone = TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo");
            }

            DateTime cairoDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, cairoZone);
            return DateOnly.FromDateTime(cairoDateTime);
        }
    }
}
