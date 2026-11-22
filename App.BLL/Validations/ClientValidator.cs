using App.DAL.Entities;
using FluentValidation;

namespace App.BLL.Validations;

public class ClientValidator : AbstractValidator<Client>
{
    public ClientValidator()
    {
        RuleFor(c => c.CompanyName)
            .NotEmpty().WithMessage("Company Name is required.")
            .MaximumLength(150).WithMessage("Company Name cannot exceed 150 characters.");

        RuleFor(c => c.Phone)
            .NotEmpty().WithMessage("Phone number is required.")
            .MaximumLength(50).WithMessage("Phone cannot exceed 50 characters.");

        RuleFor(c => c.Address)
            .NotEmpty().WithMessage("Address is required.")
            .MaximumLength(255).WithMessage("Address cannot exceed 255 characters.");
    }
}
