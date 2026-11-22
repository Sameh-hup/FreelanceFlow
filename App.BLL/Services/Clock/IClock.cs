namespace App.BLL.Services.Clock;

public interface IClock
{
    DateTime UtcNow { get; }
    DateOnly UtcToday { get; }
}
