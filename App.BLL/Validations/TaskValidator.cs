using App.DAL.Entities;
using FluentValidation;

namespace App.BLL.Validations;

public class TaskValidator : AbstractValidator<TaskItem>
{
    public TaskValidator()
    {
        RuleFor(t => t.Title)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Task title is required.");

        RuleFor(t => t.Priority)
            .IsInEnum()
            .WithMessage("Please select a valid priority.");

        RuleFor(t => t.DueDate)
            .NotEmpty()
            .WithMessage("Due date is required.");
    }
}
