using App.DAL.Entities;
using App.DAL.Enums;

namespace App.BLL.Interfaces.Services;

public interface IInvoiceService
{
    Task<List<Invoice>> GetInvoicesAsync(string freelancerId, string? searchName = null);
    Task<Invoice?> GetInvoiceDetailsAsync(int invoiceId);
    Task<bool> CreateInvoiceAsync(Invoice invoice, string freelancerId);
    Task<bool> DeleteInvoiceAsync(int invoiceId, string freelancerId);
    Task<bool> RecordPaymentAsync(int invoiceId, decimal amount, DateOnly paymentDate, string? notes, string freelancerId);
    decimal CalculateRemainingBalance(Invoice invoice);
    InvoiceStatus CalculateInvoiceStatus(Invoice invoice);

    // Overloads for int freelancerId
    Task<List<Invoice>> GetInvoicesAsync(int freelancerId, string? searchName = null);
    Task<bool> CreateInvoiceAsync(Invoice invoice, int freelancerId);
    Task<bool> DeleteInvoiceAsync(int invoiceId, int freelancerId);
    Task<bool> RecordPaymentAsync(int invoiceId, decimal amount, DateOnly paymentDate, string? notes, int freelancerId);
}
