using App.DAL.Entities;
using FluentValidation;

namespace App.BLL.Validations;

public class PaymentValidator : AbstractValidator<Payment>
{
    public PaymentValidator()
    {
        RuleFor(p => p.Amount)
            .GreaterThan(0)
            .WithMessage("Payment amount must be greater than zero.");

        RuleFor(p => p.PaymentDate)
            .NotEmpty()
            .WithMessage("Payment date is required.");

        RuleFor(p => p.Currency)
            .IsInEnum()
            .WithMessage("Invalid payment currency.");
    }
}
