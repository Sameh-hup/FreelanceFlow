using App.BLL.Services;
using App.BLL.Services.Clock;
using App.DAL.Entities;
using App.DAL.Enums;
using Xunit;

namespace App.BLL.Tests;

public class FakeClock : IClock
{
    public DateTime UtcNow { get; set; } = new DateTime(2026, 11, 23, 12, 0, 0, DateTimeKind.Utc);
    public DateOnly UtcToday => DateOnly.FromDateTime(UtcNow);
    public DateOnly CairoToday { get; set; } = new DateOnly(2026, 11, 23);
}

public class Developer1CalculationTests
{
    private readonly FakeClock _fakeClock;
    private readonly ProjectService _projectService;
    private readonly TaskService _taskService;

    public Developer1CalculationTests()
    {
        _fakeClock = new FakeClock();
        _projectService = new ProjectService(null!, null!, null!, _fakeClock);
        _taskService = new TaskService(null!, null!, null!, _fakeClock, null!);
    }

    [Fact]
    public void CalculateProgress_ProjectWithZeroTasks_ReturnsZero()
    {
        var project = new Project
        {
            Milestones = new List<Milestone>()
        };

        var progress = _projectService.CalculateProgress(project);

        Assert.Equal(0, progress);
    }

    [Fact]
    public void CalculateProgress_ThreeCompletedOutOfFourTasks_Returns75Percent()
    {
        var project = new Project
        {
            Milestones = new List<Milestone>
            {
                new Milestone
                {
                    Tasks = new List<TaskItem>
                    {
                        new TaskItem { Status = TaskItemStatus.Completed },
                        new TaskItem { Status = TaskItemStatus.Completed },
                        new TaskItem { Status = TaskItemStatus.Completed },
                        new TaskItem { Status = TaskItemStatus.Pending }
                    }
                }
            }
        };

        var progress = _projectService.CalculateProgress(project);

        Assert.Equal(75, progress);
    }

    [Fact]
    public void CalculateProgress_OneCompletedOutOfThreeTasks_Returns33PercentRounded()
    {
        var project = new Project
        {
            Milestones = new List<Milestone>
            {
                new Milestone
                {
                    Tasks = new List<TaskItem>
                    {
                        new TaskItem { Status = TaskItemStatus.Completed },
                        new TaskItem { Status = TaskItemStatus.Pending },
                        new TaskItem { Status = TaskItemStatus.InProgress }
                    }
                }
            }
        };

        var progress = _projectService.CalculateProgress(project);

        Assert.Equal(33, progress);
    }

    [Fact]
    public void IsTaskOverdue_DueDateYesterdayAndStatusPending_ReturnsTrue()
    {
        var task = new TaskItem
        {
            DueDate = _fakeClock.CairoToday.AddDays(-1),
            Status = TaskItemStatus.Pending
        };

        var isOverdue = _taskService.IsTaskOverdue(task);

        Assert.True(isOverdue);
    }

    [Fact]
    public void IsTaskOverdue_DueDateYesterdayAndStatusCompleted_ReturnsFalse()
    {
        var task = new TaskItem
        {
            DueDate = _fakeClock.CairoToday.AddDays(-1),
            Status = TaskItemStatus.Completed
        };

        var isOverdue = _taskService.IsTaskOverdue(task);

        Assert.False(isOverdue);
    }

    [Fact]
    public void IsTaskOverdue_DueDateTodayCairoTimeAndStatusPending_ReturnsFalse()
    {
        var task = new TaskItem
        {
            DueDate = _fakeClock.CairoToday,
            Status = TaskItemStatus.Pending
        };

        var isOverdue = _taskService.IsTaskOverdue(task);

        Assert.False(isOverdue);
    }

    [Fact]
    public void CalculateMilestoneStatus_NoTasks_ReturnsPending()
    {
        var milestone = new Milestone
        {
            Tasks = new List<TaskItem>()
        };

        var status = _taskService.CalculateMilestoneStatus(milestone);

        Assert.Equal(MilestoneStatus.Pending, status);
    }

    [Fact]
    public void CalculateMilestoneStatus_AllTasksCompleted_ReturnsCompleted()
    {
        var milestone = new Milestone
        {
            Tasks = new List<TaskItem>
            {
                new TaskItem { Status = TaskItemStatus.Completed },
                new TaskItem { Status = TaskItemStatus.Completed }
            }
        };

        var status = _taskService.CalculateMilestoneStatus(milestone);

        Assert.Equal(MilestoneStatus.Completed, status);
    }

    [Fact]
    public void CalculateMilestoneStatus_OneCompletedOnePending_ReturnsInProgress()
    {
        var milestone = new Milestone
        {
            Tasks = new List<TaskItem>
            {
                new TaskItem { Status = TaskItemStatus.Completed },
                new TaskItem { Status = TaskItemStatus.Pending }
            }
        };

        var status = _taskService.CalculateMilestoneStatus(milestone);

        Assert.Equal(MilestoneStatus.InProgress, status);
    }

    [Fact]
    public void CalculateHealth_NoTasks_ReturnsHealthy_BR31()
    {
        var project = new Project
        {
            Status = ProjectStatus.InProgress,
            Deadline = _fakeClock.CairoToday.AddDays(10),
            Milestones = new List<Milestone>()
        };

        var health = _projectService.CalculateHealth(project);

        Assert.Equal(ProjectHealth.Healthy, health);
    }

    [Fact]
    public void CalculateHealth_ThreeOverdueTasks_ReturnsCritical()
    {
        var project = new Project
        {
            Status = ProjectStatus.InProgress,
            Deadline = _fakeClock.CairoToday.AddDays(20),
            Milestones = new List<Milestone>
            {
                new Milestone
                {
                    Tasks = new List<TaskItem>
                    {
                        new TaskItem { DueDate = _fakeClock.CairoToday.AddDays(-2), Status = TaskItemStatus.Pending },
                        new TaskItem { DueDate = _fakeClock.CairoToday.AddDays(-3), Status = TaskItemStatus.Pending },
                        new TaskItem { DueDate = _fakeClock.CairoToday.AddDays(-1), Status = TaskItemStatus.Pending },
                        new TaskItem { DueDate = _fakeClock.CairoToday.AddDays(5), Status = TaskItemStatus.Pending }
                    }
                }
            }
        };

        var health = _projectService.CalculateHealth(project);

        Assert.Equal(ProjectHealth.Critical, health);
    }

    [Fact]
    public void CalculateHealth_DeadlineInTwoDaysAndProgress40Percent_ReturnsCritical()
    {
        var project = new Project
        {
            Status = ProjectStatus.InProgress,
            Deadline = _fakeClock.CairoToday.AddDays(2),
            Milestones = new List<Milestone>
            {
                new Milestone
                {
                    Tasks = new List<TaskItem>
                    {
                        new TaskItem { DueDate = _fakeClock.CairoToday.AddDays(1), Status = TaskItemStatus.Completed },
                        new TaskItem { DueDate = _fakeClock.CairoToday.AddDays(1), Status = TaskItemStatus.Completed },
                        new TaskItem { DueDate = _fakeClock.CairoToday.AddDays(1), Status = TaskItemStatus.Pending },
                        new TaskItem { DueDate = _fakeClock.CairoToday.AddDays(1), Status = TaskItemStatus.Pending },
                        new TaskItem { DueDate = _fakeClock.CairoToday.AddDays(1), Status = TaskItemStatus.Pending }
                    }
                }
            }
        };

        var health = _projectService.CalculateHealth(project);

        Assert.Equal(ProjectHealth.Critical, health);
    }

    [Fact]
    public void CalculateHealth_OneOverdueTaskDeadlineIn20Days_ReturnsAttentionRequired()
    {
        var project = new Project
        {
            Status = ProjectStatus.InProgress,
            Deadline = _fakeClock.CairoToday.AddDays(20),
            Milestones = new List<Milestone>
            {
                new Milestone
                {
                    Tasks = new List<TaskItem>
                    {
                        new TaskItem { DueDate = _fakeClock.CairoToday.AddDays(-1), Status = TaskItemStatus.Pending },
                        new TaskItem { DueDate = _fakeClock.CairoToday.AddDays(1), Status = TaskItemStatus.Completed },
                        new TaskItem { DueDate = _fakeClock.CairoToday.AddDays(2), Status = TaskItemStatus.Completed },
                        new TaskItem { DueDate = _fakeClock.CairoToday.AddDays(3), Status = TaskItemStatus.Completed },
                        new TaskItem { DueDate = _fakeClock.CairoToday.AddDays(4), Status = TaskItemStatus.Completed }
                    }
                }
            }
        };

        var health = _projectService.CalculateHealth(project);

        Assert.Equal(ProjectHealth.AttentionRequired, health);
    }

    [Fact]
    public void CalculateHealth_ZeroOverdueTasksDeadlineIn20DaysProgress80Percent_ReturnsHealthy()
    {
        var project = new Project
        {
            Status = ProjectStatus.InProgress,
            Deadline = _fakeClock.CairoToday.AddDays(20),
            Milestones = new List<Milestone>
            {
                new Milestone
                {
                    Tasks = new List<TaskItem>
                    {
                        new TaskItem { DueDate = _fakeClock.CairoToday.AddDays(10), Status = TaskItemStatus.Pending },
                        new TaskItem { DueDate = _fakeClock.CairoToday.AddDays(1), Status = TaskItemStatus.Completed },
                        new TaskItem { DueDate = _fakeClock.CairoToday.AddDays(2), Status = TaskItemStatus.Completed },
                        new TaskItem { DueDate = _fakeClock.CairoToday.AddDays(3), Status = TaskItemStatus.Completed },
                        new TaskItem { DueDate = _fakeClock.CairoToday.AddDays(4), Status = TaskItemStatus.Completed }
                    }
                }
            }
        };

        var health = _projectService.CalculateHealth(project);

        Assert.Equal(ProjectHealth.Healthy, health);
    }
}
