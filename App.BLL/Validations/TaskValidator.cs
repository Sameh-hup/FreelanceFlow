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
            .IsInEnum().WithMessage("Please select a valid priority.");

        RuleFor(t => t.Status)
            .IsInEnum().WithMessage("Invalid task status.");

        RuleFor(t => t.DueDate)
            .NotEmpty().WithMessage("Due date is required.");
    }
}
