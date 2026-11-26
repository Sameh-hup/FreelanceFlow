using App.BLL.Services;
using App.BLL.Services.Clock;
using App.DAL.Entities;
using App.DAL.Enums;
using Xunit;

namespace App.BLL.Tests;

public class TestClock : IClock
{
    public DateTime UtcNow { get; set; } = new DateTime(2026, 11, 25, 12, 0, 0, DateTimeKind.Utc);
    public DateOnly UtcToday => DateOnly.FromDateTime(UtcNow);
    public DateOnly CairoToday { get; set; } = new DateOnly(2026, 11, 25);
}

public class InvoiceCalculationTests
{
    private readonly TestClock _clock;
    private readonly InvoiceService _invoiceService;

    public InvoiceCalculationTests()
    {
        _clock = new TestClock { CairoToday = new DateOnly(2026, 11, 25) };
        // Pure calculation functions only rely on _clock; other dependencies are unused
        _invoiceService = new InvoiceService(null!, null!, null!, _clock);
    }

    [Fact]
    public void CalculateRemainingBalance_WhenPaymentsLessThanAmount_ReturnsRemaining()
    {
        // Arrange
        var invoice = new Invoice
        {
            Amount = 1000m,
            Payments = new List<Payment>
            {
                new Payment { Amount = 400m }
            }
        };

        // Act
        var remaining = _invoiceService.CalculateRemainingBalance(invoice);

        // Assert
        Assert.Equal(600m, remaining);
    }

    [Fact]
    public void CalculateRemainingBalance_WhenFullyPaid_ReturnsZero()
    {
        // Arrange
        var invoice = new Invoice
        {
            Amount = 1000m,
            Payments = new List<Payment>
            {
                new Payment { Amount = 1000m }
            }
        };

        // Act
        var remaining = _invoiceService.CalculateRemainingBalance(invoice);

        // Assert
        Assert.Equal(0m, remaining);
    }

    [Fact]
    public void CalculateRemainingBalance_WhenOverpaid_ClampsToZero()
    {
        // Arrange
        var invoice = new Invoice
        {
            Amount = 1000m,
            Payments = new List<Payment>
            {
                new Payment { Amount = 1200m }
            }
        };

        // Act
        var remaining = _invoiceService.CalculateRemainingBalance(invoice);

        // Assert
        Assert.Equal(0m, remaining);
    }

    [Fact]
    public void CalculateInvoiceStatus_WhenAmount1000AndPaid1000_ReturnsPaid()
    {
        // Arrange: Amount 1000, Paid 1000 -> Paid
        var invoice = new Invoice
        {
            Amount = 1000m,
            DueDate = new DateOnly(2026, 11, 20), // Even if in past, Paid takes precedence
            Payments = new List<Payment>
            {
                new Payment { Amount = 1000m }
            }
        };

        // Act
        var status = _invoiceService.CalculateInvoiceStatus(invoice);

        // Assert
        Assert.Equal(InvoiceStatus.Paid, status);
    }

    [Fact]
    public void CalculateInvoiceStatus_WhenAmount1000Paid400DueDateInFuture_ReturnsPartiallyPaid()
    {
        // Arrange: Amount 1000, Paid 400, DueDate in future -> PartiallyPaid
        var invoice = new Invoice
        {
            Amount = 1000m,
            DueDate = new DateOnly(2026, 11, 30), // In future relative to CairoToday (2026-11-25)
            Payments = new List<Payment>
            {
                new Payment { Amount = 400m }
            }
        };

        // Act
        var status = _invoiceService.CalculateInvoiceStatus(invoice);

        // Assert
        Assert.Equal(InvoiceStatus.PartiallyPaid, status);
    }

    [Fact]
    public void CalculateInvoiceStatus_WhenAmount1000Paid0DueDateInPast_ReturnsOverdue()
    {
        // Arrange: Amount 1000, Paid 0, DueDate in past -> Overdue
        var invoice = new Invoice
        {
            Amount = 1000m,
            DueDate = new DateOnly(2026, 11, 20), // Strictly in past relative to CairoToday (2026-11-25)
            Payments = new List<Payment>()
        };

        // Act
        var status = _invoiceService.CalculateInvoiceStatus(invoice);

        // Assert
        Assert.Equal(InvoiceStatus.Overdue, status);
    }

    [Fact]
    public void CalculateInvoiceStatus_WhenAmount1000Paid0DueDateInFuture_ReturnsUnpaid()
    {
        // Arrange: Amount 1000, Paid 0, DueDate in future -> Unpaid
        var invoice = new Invoice
        {
            Amount = 1000m,
            DueDate = new DateOnly(2026, 11, 30), // In future relative to CairoToday (2026-11-25)
            Payments = new List<Payment>()
        };

        // Act
        var status = _invoiceService.CalculateInvoiceStatus(invoice);

        // Assert
        Assert.Equal(InvoiceStatus.Unpaid, status);
    }
}
