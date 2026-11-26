using App.DAL.Entities;
using FluentValidation;

namespace App.BLL.Validations;

public class ProjectValidator : AbstractValidator<Project>
{
    public ProjectValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("Project Name is required.")
            .MaximumLength(150).WithMessage("Project Name cannot exceed 150 characters.");

        RuleFor(p => p.Budget)
            .GreaterThan(0).WithMessage("Project Budget must be greater than zero.");

        RuleFor(p => p.Deadline)
            .GreaterThan(p => p.StartDate)
            .WithMessage("Project Deadline must be strictly after the Start Date (BR-01).");
    }
}
