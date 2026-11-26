using App.DAL.Entities;
using FluentValidation;

namespace App.BLL.Validations;

public class InvoiceValidator : AbstractValidator<Invoice>
{
    public InvoiceValidator()
    {
        RuleFor(i => i.ProjectID)
            .GreaterThan(0)
            .WithMessage("Please link the invoice to a project.");

        RuleFor(i => i.Amount)
            .GreaterThan(0)
            .WithMessage("Invoice amount must be greater than zero.");

        RuleFor(i => i.IssueDate)
            .NotEmpty()
            .WithMessage("Issue date is required.");

        RuleFor(i => i.DueDate)
            .GreaterThanOrEqualTo(i => i.IssueDate)
            .WithMessage("Invoice Due Date must be on or after the Issue Date (BR-15).");

        RuleFor(i => i.Currency)
            .IsInEnum()
            .WithMessage("Invalid currency.");
    }
}
