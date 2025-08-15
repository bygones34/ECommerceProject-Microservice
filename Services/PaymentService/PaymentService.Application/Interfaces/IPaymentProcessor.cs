using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Application.Interfaces
{
    public interface IPaymentProcessor
    {
        Task<(bool success, string providerRef, string? reason)> ChargeAsync(
            Guid orderId, string userName, decimal amount, string currency, CancellationToken ct);
    }
}
