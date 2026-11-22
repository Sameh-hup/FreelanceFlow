using System.ComponentModel.DataAnnotations;

namespace App.DAL.Validations;

public class NoFutureDate : ValidationAttribute
{
    public NoFutureDate()
    {
        ErrorMessage = "Date cannot be in the future.";
    }

    public override bool IsValid(object? value)
    {
        if (value is null)
            return true;

        if (value is DateOnly dateOnly)
            return dateOnly <= DateOnly.FromDateTime(DateTime.UtcNow);

        if (value is DateTime dateTime)
            return dateTime <= DateTime.UtcNow;

        return true;
    }
}
