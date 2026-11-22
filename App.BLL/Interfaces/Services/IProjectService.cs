using App.DAL.Entities;
using App.DAL.Enums;

namespace App.BLL.Interfaces.Services;

public interface IProjectService
{
    Task<List<Project>> GetProjectsAsync(int freelancerId, string? searchName = null);
    Task<Project?> GetProjectDetailsAsync(int projectId, int freelancerId);
    Task<bool> CreateProjectAsync(Project project, int freelancerId);
    Task<bool> UpdateProjectStatusAsync(int projectId, ProjectStatus newStatus, int freelancerId);
    Task<bool> DeleteProjectAsync(int projectId, int freelancerId);
    int CalculateProgress(Project project);
    ProjectHealth CalculateHealth(Project project);
}
