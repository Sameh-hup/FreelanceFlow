using App.DAL.Entities;
using App.DAL.Enums;
using FluentValidation;

namespace App.BLL.Validations;

public class ProjectValidator : AbstractValidator<Project>
{
    public ProjectValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(150)
            .WithMessage("Project name is required.");

        RuleFor(p => p.ClientID)
            .GreaterThan(0)
            .WithMessage("Please select a valid client.");

        RuleFor(p => p.Budget)
            .GreaterThan(0)
            .WithMessage("Budget must be greater than zero.");

        RuleFor(p => p.Currency)
            .Must(c => c == Currency.EGP || c == Currency.USD || c == Currency.EUR)
            .WithMessage("Currency must be EGP, USD, or EUR.");

        RuleFor(p => p.StartDate)
            .NotEmpty()
            .WithMessage("Start date is required.");

        RuleFor(p => p.Deadline)
            .GreaterThan(p => p.StartDate)
            .WithMessage("Deadline must be after start date.");
    }
}
