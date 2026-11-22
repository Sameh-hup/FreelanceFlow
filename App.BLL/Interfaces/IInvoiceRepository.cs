using App.DAL.Entities;

namespace App.BLL.Interfaces;

public interface IInvoiceRepository : IRepository<Invoice>
{
    Task<List<Invoice>> GetInvoicesByFreelancerIdAsync(string freelancerId);
    Task<Invoice?> GetInvoiceWithPaymentsAsync(int invoiceId);
    Task<List<Invoice>> GetInvoicesByClientIdAsync(int clientId);
}
