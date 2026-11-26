using App.DAL.Entities;
using FluentValidation;

namespace App.BLL.Validations;

public class InvoiceValidator : AbstractValidator<Invoice>
{
    public InvoiceValidator()
    {
        RuleFor(i => i.Amount)
            .GreaterThan(0).WithMessage("Invoice Amount must be greater than zero.");

        RuleFor(i => i.DueDate)
            .GreaterThanOrEqualTo(i => i.IssueDate)
            .WithMessage("Invoice Due Date must be on or after the Issue Date (BR-15).");

        RuleFor(i => i.Currency)
            .IsInEnum().WithMessage("Invalid currency.");
    }
}
