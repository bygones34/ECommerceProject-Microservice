namespace ECommerce.Contracts.Payments;

public sealed record PaymentSucceededEvent(
    Guid OrderId,
    string UserName,
    decimal Amount,
    string Currency,
    DateTime PaidAt);