using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minipay.Domain.Payments;

namespace Minipay.Application.Commons.Interfaces
{
    public interface IPaymentRepository
    {

        // new payment
        Task AddAsync(Payment payment, CancellationToken cancellationToken = default);

        // returns the payment by id or null if not found
        Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        // returns all payments with the given status// inumerable gives a collection of payments.
        Task<IEnumerable<Payment>> GetByStatusAsync(PaymentStatus status, CancellationToken cancellationToken = default);

        // returns changes made to an existing payment
        Task UpdateAsync(Payment payment, CancellationToken cancellationToken = default);
    }
}
