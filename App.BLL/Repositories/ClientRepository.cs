using App.BLL.Interfaces;
using App.DAL.Data;
using App.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Repositories;

public class ClientRepository : Repository<Client>, IClientRepository
{
    public ClientRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<Client>> GetClientsByFreelancerIdAsync(string freelancerId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => c.UserID == freelancerId && !c.IsDeleted)
            .Include(e=>e.Projects)
            .ToListAsync();
    }

    public async Task<Client?> GetClientWithProjectsAsync(int clientId, string freelancerId)
    {
        return await _dbSet
            .Include(c => c.PortalUser)
            .Include(c => c.Projects)
                .ThenInclude(p => p.Milestones)
                    .ThenInclude(m => m.Tasks)
            .FirstOrDefaultAsync(c => c.ID == clientId && c.UserID == freelancerId && !c.IsDeleted);
    }
}
