
using Microsoft.EntityFrameworkCore;
using PaymentService.Application.Interfaces;
using PaymentService.Domain.Entities;
using PaymentService.Infrastructure.Context;

namespace PaymentService.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly PaymentDbContext _db;

        public PaymentRepository(PaymentDbContext db) => _db = db;

        public Task<Payment?> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default)
            => _db.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId, ct);

        public async Task AddAsync(Payment payment, CancellationToken ct = default)
            => await _db.Payments.AddAsync(payment, ct);

        public Task UpdateAsync(Payment payment, CancellationToken ct = default)
        {
            _db.Payments.Update(payment);
            return Task.CompletedTask;
        }

        public async Task<bool> SaveChangesAsync(CancellationToken ct = default)
            => (await _db.SaveChangesAsync(ct)) > 0;
    }
}
