using App.DAL.Entities;
using FluentValidation;

namespace App.BLL.Validations;

public class PaymentValidator : AbstractValidator<Payment>
{
    public PaymentValidator()
    {
        RuleFor(p => p.Amount)
            .GreaterThan(0).WithMessage("Payment Amount must be greater than zero.");

        RuleFor(p => p.Currency)
            .IsInEnum().WithMessage("Invalid payment currency.");
    }
}
