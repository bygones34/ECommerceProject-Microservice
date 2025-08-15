namespace ECommerce.Contracts.Payments;

public sealed record PaymentFailedEvent(
    Guid OrderId,
    string UserName,
    decimal Amount,
    string Currency,
    string Reason,
    DateTime FailedAt);
