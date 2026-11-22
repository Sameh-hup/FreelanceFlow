using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace App.DAL.Validations;

public class AfterDate : ValidationAttribute
{
    private readonly string _otherPropertyName;

    public AfterDate(string otherPropertyName)
    {
        _otherPropertyName = otherPropertyName;
        ErrorMessage = $"Must be on or after {_otherPropertyName}.";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return ValidationResult.Success;

        PropertyInfo? otherProperty = validationContext.ObjectType.GetProperty(_otherPropertyName);
        if (otherProperty is null)
            return new ValidationResult($"Unknown property: {_otherPropertyName}");

        object? otherValue = otherProperty.GetValue(validationContext.ObjectInstance);
        if (otherValue is null)
            return ValidationResult.Success;

        // DateOnly comparison
        if (value is DateOnly dateOnlyVal && otherValue is DateOnly otherDateOnly)
        {
            if (dateOnlyVal < otherDateOnly)
                return new ValidationResult(ErrorMessageString);
            return ValidationResult.Success;
        }

        // DateTime comparison
        if (value is DateTime dateTimeVal && otherValue is DateTime otherDateTime)
        {
            if (dateTimeVal < otherDateTime)
                return new ValidationResult(ErrorMessageString);
            return ValidationResult.Success;
        }

        return ValidationResult.Success;
    }
}
