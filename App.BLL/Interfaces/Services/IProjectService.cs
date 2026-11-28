using App.DAL.Entities;
using App.DAL.Enums;

namespace App.BLL.Interfaces.Services;

public interface IProjectService
{
    Task<List<Project>> GetProjectsAsync(string freelancerId, string? searchName = null);
    Task<Project?> GetProjectDetailsAsync(int projectId, string freelancerId);
    Task<bool> CreateProjectAsync(Project project, string freelancerId);
    Task<bool> UpdateProjectStatusAsync(int projectId, ProjectStatus newStatus, string freelancerId);
    Task<bool> DeleteProjectAsync(int projectId, string freelancerId);
    int CalculateProgress(Project project);
    ProjectHealth CalculateHealth(Project project);

    }
