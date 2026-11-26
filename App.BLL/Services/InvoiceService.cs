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
    private readonly AppDbContext _context;
    private readonly IAccessService _accessService;
    private readonly IClock _clock;

    public InvoiceService(
        IInvoiceRepository invoiceRepository,
        AppDbContext context,
        IAccessService accessService,
        IClock clock)
    {
        _invoiceRepository = invoiceRepository;
        _context = context;
        _accessService = accessService;
        _clock = clock;
    }

    public async Task<List<Invoice>> GetInvoicesAsync(string freelancerId, string? searchName = null)
    {
        var invoices = await _invoiceRepository.GetInvoicesByFreelancerIdAsync(freelancerId);

        if (!string.IsNullOrWhiteSpace(searchName))
        {
            invoices = invoices.Where(i =>
                (i.Project?.Name?.Contains(searchName, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (i.Project?.Client?.CompanyName?.Contains(searchName, StringComparison.OrdinalIgnoreCase) ?? false)
            ).ToList();
        }

        return invoices;
    }

    public async Task<Invoice?> GetInvoiceDetailsAsync(int invoiceId, string freelancerId)
    {
        var invoice = await _invoiceRepository.GetInvoiceWithPaymentsAsync(invoiceId);
        if (invoice == null)
            return null;

        if (!string.IsNullOrEmpty(freelancerId))
        {
            var hasAccess = await _accessService.CanFreelancerAccessInvoiceAsync(freelancerId, invoiceId);
            if (!hasAccess)
                throw new UnauthorizedAccessException("You do not have access to this invoice.");
        }

        return invoice;
    }

    public async Task<bool> CreateInvoiceAsync(Invoice invoice, string freelancerId)
    {
        var hasAccess = await _accessService.CanFreelancerAccessProjectAsync(freelancerId, invoice.ProjectID);
        if (!hasAccess)
            throw new UnauthorizedAccessException("You do not have access to create an invoice for this project.");

        if (invoice.Amount <= 0)
            throw new ArgumentException("Invoice amount must be greater than zero.");

        if (invoice.DueDate < invoice.IssueDate)
            throw new ArgumentException("Due date must be on or after issue date.");

        // Lock currency to project currency (BR-08)
        var project = await _context.Projects.FindAsync(invoice.ProjectID);
        if (project == null)
            throw new InvalidOperationException("Project not found.");

        invoice.Currency = project.Currency;
        invoice.CreatedAtUtc = _clock.UtcNow;

        await _invoiceRepository.AddAsync(invoice);
        await _invoiceRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteInvoiceAsync(int invoiceId, string freelancerId)
    {
        var invoice = await _invoiceRepository.GetInvoiceWithPaymentsAsync(invoiceId);
        if (invoice == null)
            return false;

        if (!string.IsNullOrEmpty(freelancerId))
        {
            var hasAccess = await _accessService.CanFreelancerAccessInvoiceAsync(freelancerId, invoiceId);
            if (!hasAccess)
                throw new UnauthorizedAccessException("You do not have access to delete this invoice.");
        }

        // BR-21: Check if any payment exists. If invoice.Payments.Any(), deletion is strictly forbidden.
        if (invoice.Payments.Any())
            throw new InvalidOperationException("Invoices with existing payments cannot be deleted.");

        _invoiceRepository.Delete(invoice);
        await _invoiceRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RecordPaymentAsync(int invoiceId, decimal amount, DateOnly paymentDate, string? notes, string freelancerId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 2. Load the invoice fresh from the database including existing payments (do not rely on cached screen values)
            var invoice = await _context.Invoices
                .Include(i => i.Payments)
                .Include(i => i.Project)
                    .ThenInclude(p => p.Client)
                .FirstOrDefaultAsync(i => i.ID == invoiceId);

            if (invoice == null)
                throw new KeyNotFoundException("Invoice not found.");

            if (!string.IsNullOrEmpty(freelancerId) &&
                (invoice.Project?.Client?.UserID != freelancerId || (invoice.Project?.Client?.IsDeleted ?? false)))
            {
                throw new UnauthorizedAccessException("You do not have permission to record payments on this invoice.");
            }

            // 3. Calculate current remaining balance
            decimal balance = CalculateRemainingBalance(invoice);

            // 4. Validate Amount
            if (amount <= 0 || amount > balance)
            {
                throw new ArgumentException("Payment amount cannot exceed remaining balance and must be greater than zero.");
            }

            // 5. Validate PaymentDate (BR-20: no future payments)
            if (paymentDate > _clock.CairoToday)
            {
                throw new ArgumentException("Payment date cannot be in the future.");
            }

            // 6. Insert new Payment
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

            // 7. Write AuditLog entry inside the same transaction (BR-19, BR-29)
            var auditLog = new AuditLog
            {
                EntityName = "Payment",
                Action = "Create",
                UserID = freelancerId,
                EntityID = invoiceId,
                CreatedAtUtc = _clock.UtcNow,
                NewValues = $"Amount: {amount}, Currency: {invoice.Currency}, PaymentDate: {paymentDate}"
            };
            await _context.AuditLogs.AddAsync(auditLog);

            // 8. Try SaveChangesAsync with concurrency handling
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync();
            throw new InvalidOperationException("The invoice was updated concurrently, please try again.");
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public decimal CalculateRemainingBalance(Invoice invoice)
    {
        decimal totalPayments = invoice.Payments?.Sum(p => p.Amount) ?? 0m;
        decimal remaining = invoice.Amount - totalPayments;
        return remaining < 0m ? 0m : remaining;
    }

    public InvoiceStatus CalculateInvoiceStatus(Invoice invoice)
    {
        // 1. CalculateRemainingBalance(invoice) == 0 -> Paid
        if (CalculateRemainingBalance(invoice) == 0m)
            return InvoiceStatus.Paid;

        // 2. invoice.DueDate < _clock.CairoToday -> Overdue
        if (invoice.DueDate < _clock.CairoToday)
            return InvoiceStatus.Overdue;

        // 3. invoice.Payments.Any() (Total payments > 0) -> PartiallyPaid
        if (invoice.Payments != null && invoice.Payments.Any(p => p.Amount > 0))
            return InvoiceStatus.PartiallyPaid;

        // 4. Default -> Unpaid
        return InvoiceStatus.Unpaid;
    }
}
