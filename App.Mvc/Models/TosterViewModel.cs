namespace App.Mvc.Models;

public enum TosterType
{
    Success,
    Error,
    Warning,
    Info
}

public class TosterViewModel
{
    public TosterType Type { get; set; } = TosterType.Success;
    public string Content { get; set; } = "Operation completed successfully.";
}
