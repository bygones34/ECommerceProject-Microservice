using PaymentService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Application.Payments
{
    public class FakePaymentProcessor : IPaymentProcessor
    {
        public Task<(bool success, string providerRef, string? reason)> ChargeAsync(
        Guid orderId, string userName, decimal amount, string currency, CancellationToken ct)
        {
            var ok = Random.Shared.Next(1, 101) <= 90;
            if (ok)
                return Task.FromResult((true, $"REF-{orderId.ToString()[..8]}", (string?)null));

            return Task.FromResult((false, "", "Rejected by Banking"));
        }
    }
}
