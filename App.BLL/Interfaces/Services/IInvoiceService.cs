using App.DAL.Entities;
using App.DAL.Enums;

namespace App.BLL.Interfaces.Services;

public interface IInvoiceService
{
    Task<List<Invoice>> GetInvoicesAsync(int freelancerId, string? searchName = null);
    Task<Invoice?> GetInvoiceDetailsAsync(int invoiceId);
    Task<bool> CreateInvoiceAsync(Invoice invoice, int freelancerId);
    Task<bool> DeleteInvoiceAsync(int invoiceId, int freelancerId);
    Task<bool> RecordPaymentAsync(int invoiceId, decimal amount, DateOnly paymentDate, string? notes, int freelancerId);
    decimal CalculateRemainingBalance(Invoice invoice);
    InvoiceStatus CalculateInvoiceStatus(Invoice invoice);
}
