using App.BLL.Interfaces;
using App.DAL.Data;
using App.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Repositories;

public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
{
    public InvoiceRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<Invoice>> GetInvoicesByFreelancerIdAsync(string freelancerId)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(i => i.Project)
                .ThenInclude(p => p.Client)
            .Include(i => i.Payments)
            .Where(i => i.Project.Client.UserID == freelancerId && !i.Project.Client.IsDeleted)
            .ToListAsync();
    }

    public async Task<Invoice?> GetInvoiceWithPaymentsAsync(int invoiceId)
    {
        return await _dbSet
            .Include(i => i.Project)
                .ThenInclude(p => p.Client)
            .Include(i => i.Payments)
            .FirstOrDefaultAsync(i => i.ID == invoiceId);
    }

    public async Task<List<Invoice>> GetInvoicesByClientIdAsync(int clientId)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(i => i.Project)
            .Include(i => i.Payments)
            .Where(i => i.Project.ClientID == clientId && !i.Project.Client.IsDeleted)
            .ToListAsync();
    }
}
