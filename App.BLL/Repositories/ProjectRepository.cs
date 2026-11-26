using App.BLL.Interfaces;
using App.DAL.Data;
using App.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Repositories;

public class ProjectRepository : Repository<Project>, IProjectRepository
{
    public ProjectRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<Project>> GetProjectsByFreelancerIdAsync(string freelancerId)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(p => p.Client)
            .Include(p => p.Milestones)
                .ThenInclude(m => m.Tasks)
            .Include(p => p.Invoices)
            .Where(p => p.Client.UserID == freelancerId && !p.Client.IsDeleted)
            .ToListAsync();
    }

    public async Task<Project?> GetProjectDetailsAsync(int projectId, string freelancerId)
    {
        var project = await _dbSet
            .Include(p => p.Client)
            .Include(p => p.Milestones)
                .ThenInclude(m => m.Tasks)
            .Include(p => p.Invoices)
            .FirstOrDefaultAsync(p => p.ID == projectId && p.Client.UserID == freelancerId && !p.Client.IsDeleted);

        if (project != null)
        {
            project.Milestones = project.Milestones
                .OrderBy(m => m.SortOrder)
                .Select(m =>
                {
                    m.Tasks = m.Tasks.OrderBy(t => t.SortOrder).ToList();
                    return m;
                })
                .ToList();
        }

        return project;
    }

    public async Task<List<Project>> GetProjectsByClientIdAsync(int clientId)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(p => p.Milestones)
                .ThenInclude(m => m.Tasks)
            .Include(p => p.Invoices)
            .Where(p => p.ClientID == clientId && !p.Client.IsDeleted)
            .ToListAsync();
    }
}
