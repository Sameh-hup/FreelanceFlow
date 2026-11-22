namespace App.BLL.Services.Clock;

public interface IClock
{
    DateTime UtcNow { get; }
    DateTime CairoNow { get; }
    DateOnly TodayInCairo { get; }
}
