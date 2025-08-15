using PaymentService.Domain.Enums;

namespace PaymentService.Domain.Entities;

public class Payment
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // Order
    public Guid OrderId { get; private set; }

    // ID / Customer
    public string UserName { get; private set; } = null!;

    // Amount
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "TRY";

    // Status
    public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;
    public string? ProviderReference { get; private set; }

    // Tracking
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    private Payment() { } // For EF

    public Payment(Guid orderId, string userName, decimal amount, string currency = "TRY")
    {
        if (orderId == Guid.Empty) throw new ArgumentException("OrderId boş olamaz", nameof(orderId));
        if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentException("UserName boş olamaz", nameof(userName));
        if (amount <= 0) throw new ArgumentException("Amount 0'dan büyük olmalı", nameof(amount));
        if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Currency boş olamaz", nameof(currency));

        OrderId = orderId;
        UserName = userName;
        Amount = amount;
        Currency = currency;
        Status = PaymentStatus.Pending;
    }

    public void MarkPending()
    {
        Status = PaymentStatus.Pending;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkSucceeded(string providerRef)
    {
        if (string.IsNullOrWhiteSpace(providerRef))
            throw new ArgumentException("Provider reference boş olamaz", nameof(providerRef));

        Status = PaymentStatus.Succeeded;
        ProviderReference = providerRef;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkFailed(string? reason = null)
    {
        Status = PaymentStatus.Failed;
        UpdatedAt = DateTime.UtcNow;
    }
}
