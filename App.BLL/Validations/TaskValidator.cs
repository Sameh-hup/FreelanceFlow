using App.DAL.Entities;
using FluentValidation;

namespace App.BLL.Validations;

public class TaskValidator : AbstractValidator<TaskItem>
{
    public TaskValidator()
    {
        RuleFor(t => t.Title)
            .NotEmpty().WithMessage("Task title is required.")
            .MaximumLength(200).WithMessage("Task title cannot exceed 200 characters.");

        RuleFor(t => t.Priority)
            .IsInEnum().WithMessage("Invalid task priority.");

        RuleFor(t => t.Status)
            .IsInEnum().WithMessage("Invalid task status.");
    }
}
