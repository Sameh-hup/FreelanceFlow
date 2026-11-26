using App.DAL.Entities;
using FluentValidation;

namespace App.BLL.Validations;

public class ClientValidator : AbstractValidator<Client>
{
    public ClientValidator()
    {
        RuleFor(c => c.CompanyName)
            .NotEmpty()
            .MaximumLength(150)
            .WithMessage("Company name is required and cannot exceed 150 characters.");

        RuleFor(c => c.Phone)
            .NotEmpty()
            .Matches(@"^[0-9+\s-]{8,20}$")
            .WithMessage("Please enter a valid phone number.");

        RuleFor(c => c.Address)
            .MaximumLength(250)
            .WithMessage("Address cannot exceed 250 characters.");
    }
}
