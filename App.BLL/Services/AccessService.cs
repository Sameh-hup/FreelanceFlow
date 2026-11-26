using App.BLL.Interfaces.Services;
using App.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Services;

public class AccessService : IAccessService
{
    private readonly AppDbContext _context;

    public AccessService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CanFreelancerAccessClientAsync(string freelancerId, int clientId)
    {
        return await _context.Clients
            .AnyAsync(c => c.ID == clientId && c.UserID == freelancerId && !c.IsDeleted);
    }

    public async Task<bool> CanFreelancerAccessProjectAsync(string freelancerId, int projectId)
    {
        return await _context.Projects
            .Include(p => p.Client)
            .AnyAsync(p => p.ID == projectId && p.Client.UserID == freelancerId && !p.Client.IsDeleted);
    }

    public async Task<bool> CanFreelancerAccessInvoiceAsync(string freelancerId, int invoiceId)
    {
        return await _context.Invoices
            .Include(i => i.Project)
                .ThenInclude(p => p.Client)
            .AnyAsync(i => i.ID == invoiceId && i.Project.Client.UserID == freelancerId && !i.Project.Client.IsDeleted);
    }

    public async Task<bool> CanClientAccessProjectAsync(string clientUserId, int projectId)
    {
        var user = await _context.Users.FindAsync(clientUserId);
        if (user == null || user.ClientId == null)
            return false;

        return await _context.Projects
            .Include(p => p.Client)
            .AnyAsync(p => p.ID == projectId && p.ClientID == user.ClientId && !p.Client.IsDeleted);
    }
}
