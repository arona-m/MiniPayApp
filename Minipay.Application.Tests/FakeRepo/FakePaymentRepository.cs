using Minipay.Application.Commons.Interfaces;
using Minipay.Domain.Payments;

namespace Minipay.Application.Tests.FakeRepo
{
    public sealed class FakePaymentRepository : IPaymentRepository
    {
        private readonly Dictionary<Guid, Payment> _payments = new();

        public IReadOnlyCollection<Payment> Payments => _payments.Values.ToList();

        public Task AddAsync(Payment payment, CancellationToken cancellationToken = default)
        {
            _payments[payment.Id] = payment;
            return Task.CompletedTask;
        }

        public Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_payments.GetValueOrDefault(id));

        public Task<IEnumerable<Payment>> GetByStatusAsync(PaymentStatus status, CancellationToken cancellationToken = default)
            => Task.FromResult<IEnumerable<Payment>>(_payments.Values.Where(p => p.Status == status).ToList());

        public Task UpdateAsync(Payment payment, CancellationToken cancelationToken = default)
        {
            _payments[payment.Id] = payment;
            return Task.CompletedTask;
        }
    }
}
