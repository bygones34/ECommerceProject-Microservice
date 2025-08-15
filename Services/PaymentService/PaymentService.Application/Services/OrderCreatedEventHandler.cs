using MassTransit;
using Microsoft.Extensions.Logging;
using ECommerce.Contracts.Orders;
using PaymentService.Application.Interfaces;
using PaymentService.Domain.Entities;
using ECommerce.Contracts.Payments;

namespace PaymentService.Application.Services;

public class OrderCreatedEventHandler : IOrderCreatedEventHandler
{
    private readonly IPaymentRepository _repo;
    private readonly IPaymentProcessor _processor;
    private readonly IPublishEndpoint _publish;
    private readonly ILogger<OrderCreatedEventHandler> _logger;

    public OrderCreatedEventHandler(
        IPaymentRepository repo,
        IPaymentProcessor processor,
        IPublishEndpoint publish,
        ILogger<OrderCreatedEventHandler> logger)
    {
        _repo = repo;
        _processor = processor;
        _publish = publish;
        _logger = logger;
    }

    public async Task HandleAsync(OrderCreatedEvent evt, Guid messageId, CancellationToken ct)
    {
        _logger.LogInformation("OrderCreatedEvent alındı. OrderId={OrderId}", evt.OrderId);

        var existing = await _repo.GetByOrderIdAsync(evt.OrderId, ct);
        if (existing != null)
        {
            _logger.LogWarning("Order zaten ödenmiş veya işlenmiş. OrderId={OrderId}", evt.OrderId);
            return;
        }

        var payment = new Payment(evt.OrderId, evt.UserName, evt.TotalPrice, "TRY");
        await _repo.AddAsync(payment, ct);
        await _repo.SaveChangesAsync(ct);

        var (success, providerRef, reason) =
            await _processor.ChargeAsync(evt.OrderId, evt.UserName, evt.TotalPrice, "TRY", ct);

        if (success)
        {
            payment.MarkSucceeded(providerRef);
            await _repo.UpdateAsync(payment, ct);
            await _repo.SaveChangesAsync(ct);

            await _publish.Publish(new PaymentSucceededEvent(
                payment.OrderId, payment.UserName, payment.Amount, payment.Currency, DateTime.UtcNow), ct);

            _logger.LogInformation("Payment başarılı. OrderId={OrderId}", evt.OrderId);
        }
        else
        {
            payment.MarkFailed(reason);
            await _repo.UpdateAsync(payment, ct);
            await _repo.SaveChangesAsync(ct);

            await _publish.Publish(new PaymentFailedEvent(
                payment.OrderId, payment.UserName, payment.Amount, payment.Currency, reason ?? "Unknown", DateTime.UtcNow), ct);

            _logger.LogWarning("Payment başarısız. OrderId={OrderId}, Reason={Reason}", evt.OrderId, reason);
        }
    }
}
