using App.DAL.Entities;
using App.DAL.Enums;

namespace App.BLL.Interfaces.Services;

public interface IInvoiceService
{
    Task<List<Invoice>> GetInvoicesAsync(string freelancerId, string? searchName = null);
    Task<Invoice?> GetInvoiceDetailsAsync(int invoiceId, string freelancerId);
    Task<bool> CreateInvoiceAsync(Invoice invoice, string freelancerId);
    Task<bool> DeleteInvoiceAsync(int invoiceId, string freelancerId);
    Task<bool> RecordPaymentAsync(int invoiceId, decimal amount, DateOnly paymentDate, string? notes, string freelancerId);
    decimal CalculateRemainingBalance(Invoice invoice);
    InvoiceStatus CalculateInvoiceStatus(Invoice invoice);

    // Overloads for legacy int compatibility
    Task<List<Invoice>> GetInvoicesAsync(int freelancerId, string? searchName = null) => GetInvoicesAsync(freelancerId.ToString(), searchName);
    Task<Invoice?> GetInvoiceDetailsAsync(int invoiceId) => GetInvoiceDetailsAsync(invoiceId, string.Empty);
    Task<bool> CreateInvoiceAsync(Invoice invoice, int freelancerId) => CreateInvoiceAsync(invoice, freelancerId.ToString());
    Task<bool> DeleteInvoiceAsync(int invoiceId, int freelancerId) => DeleteInvoiceAsync(invoiceId, freelancerId.ToString());
    Task<bool> RecordPaymentAsync(int invoiceId, decimal amount, DateOnly paymentDate, string? notes, int freelancerId) => RecordPaymentAsync(invoiceId, amount, paymentDate, notes, freelancerId.ToString());
}
