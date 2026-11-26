using App.BLL.Interfaces;
using App.BLL.Interfaces.Services;
using App.BLL.Services.Clock;
using App.DAL.Data;
using App.DAL.Entities;
using App.DAL.Enums;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IAccessService _accessService;
    private readonly IClock _clock;
    private readonly AppDbContext _context;

    public InvoiceService(
        IInvoiceRepository invoiceRepository,
        IAccessService accessService,
        IClock clock,
        AppDbContext context)
    {
        _invoiceRepository = invoiceRepository;
        _accessService = accessService;
        _clock = clock;
        _context = context;
    }

    public async Task<List<Invoice>> GetInvoicesAsync(string freelancerId, string? searchName = null)
    {
        var invoices = await _invoiceRepository.GetInvoicesByFreelancerIdAsync(freelancerId);
        if (!string.IsNullOrWhiteSpace(searchName))
        {
            var search = searchName.Trim();
            invoices = invoices
                .Where(i => i.Project.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                            i.Project.Client.CompanyName.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        return invoices;
    }

    public async Task<Invoice?> GetInvoiceDetailsAsync(int invoiceId)
    {
        return await _invoiceRepository.GetInvoiceWithPaymentsAsync(invoiceId);
    }

    public async Task<bool> CreateInvoiceAsync(Invoice invoice, string freelancerId)
    {
        var hasAccess = await _accessService.CanFreelancerAccessProjectAsync(freelancerId, invoice.ProjectID);
        if (!hasAccess)
            throw new UnauthorizedAccessException("You do not have permission to create an invoice for this project.");

        var project = await _context.Projects.FindAsync(invoice.ProjectID);
        if (project == null)
            throw new InvalidOperationException("Project not found.");

        // BR-08: Lock currency to project currency
        invoice.Currency = project.Currency;

        // BR-15: DueDate >= IssueDate
        if (invoice.DueDate < invoice.IssueDate)
            throw new ArgumentOutOfRangeException(nameof(invoice.DueDate), "Invoice Due Date must be on or after the Issue Date .");

        invoice.CreatedAtUtc = _clock.UtcNow;

        await _invoiceRepository.AddAsync(invoice);
        return await _invoiceRepository.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteInvoiceAsync(int invoiceId, string freelancerId)
    {
        var invoice = await _invoiceRepository.GetInvoiceWithPaymentsAsync(invoiceId);
        if (invoice == null || invoice.Project.Client.UserID != freelancerId || invoice.Project.Client.IsDeleted)
            return false;

        // BR-21: Forbid deletion if any payments exist
        if (invoice.Payments != null && invoice.Payments.Any())
        {
            throw new InvalidOperationException("Cannot delete an invoice that has recorded payments .");
        }

        _invoiceRepository.Delete(invoice);
        return await _invoiceRepository.SaveChangesAsync() > 0;
    }

    public async Task<bool> RecordPaymentAsync(int invoiceId, decimal amount, DateOnly paymentDate, string? notes, string freelancerId)
    {
        // BR-19: Atomic Transaction
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var invoice = await _context.Invoices
                .Include(i => i.Payments)
                .Include(i => i.Project)
                    .ThenInclude(p => p.Client)
                .FirstOrDefaultAsync(i => i.ID == invoiceId);

            if (invoice == null || invoice.Project.Client.UserID != freelancerId || invoice.Project.Client.IsDeleted)
                throw new UnauthorizedAccessException("Invoice not found or access denied.");

            // BR-18: Validate balance
            var remainingBalance = CalculateRemainingBalance(invoice);
            if (amount <= 0 || amount > remainingBalance)
            {
                throw new InvalidOperationException($"Payment amount ({amount:C}) cannot exceed remaining balance ({remainingBalance:C}).");
            }

            // BR-20: No future payments
            var today = _clock.UtcToday;
            if (paymentDate > today)
            {
                throw new ArgumentOutOfRangeException(nameof(paymentDate), "Payment date cannot be in the future .");
            }

            // Create Payment
            var payment = new Payment
            {
                InvoiceID = invoiceId,
                Amount = amount,
                Currency = invoice.Currency,
                PaymentDate = paymentDate,
                Notes = notes,
                CreatedAtUtc = _clock.UtcNow
            };
            await _context.Payments.AddAsync(payment);

            // BR-19 / BR-29: Audit Log inside same transaction
            var auditLog = new AuditLog
            {
                EntityName = "Payment",
                EntityID = invoiceId,
                Action = "Create",
                UserID = freelancerId,
                CreatedAtUtc = _clock.UtcNow,
                NewValues = $"Amount: {amount}, PaymentDate: {paymentDate}, Currency: {invoice.Currency}"
            };
            await _context.AuditLogs.AddAsync(auditLog);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync();
            // NFR-11: Clear concurrency conflict message
            throw new DbUpdateConcurrencyException("The invoice was updated concurrently by another operation, please try again.");
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public decimal CalculateRemainingBalance(Invoice invoice)
    {
        // BR-17
        var totalPayments = invoice.Payments?.Sum(p => p.Amount) ?? 0m;
        return Math.Max(0m, invoice.Amount - totalPayments);
    }

    public InvoiceStatus CalculateInvoiceStatus(Invoice invoice)
    {
        // BR-16: Strict sequential priority evaluation
        if (CalculateRemainingBalance(invoice) == 0m)
        {
            return InvoiceStatus.Paid;
        }

        var today = _clock.UtcToday;
        if (invoice.DueDate < today)
        {
            return InvoiceStatus.Overdue;
        }

        if (invoice.Payments != null && invoice.Payments.Any(p => p.Amount > 0))
        {
            return InvoiceStatus.PartiallyPaid;
        }

        return InvoiceStatus.Unpaid;
    }

    // Overloads for int freelancerId
    public Task<List<Invoice>> GetInvoicesAsync(int freelancerId, string? searchName = null)
        => GetInvoicesAsync(freelancerId.ToString(), searchName);

    public Task<bool> CreateInvoiceAsync(Invoice invoice, int freelancerId)
        => CreateInvoiceAsync(invoice, freelancerId.ToString());

    public Task<bool> DeleteInvoiceAsync(int invoiceId, int freelancerId)
        => DeleteInvoiceAsync(invoiceId, freelancerId.ToString());

    public Task<bool> RecordPaymentAsync(int invoiceId, decimal amount, DateOnly paymentDate, string? notes, int freelancerId)
        => RecordPaymentAsync(invoiceId, amount, paymentDate, notes, freelancerId.ToString());
}
