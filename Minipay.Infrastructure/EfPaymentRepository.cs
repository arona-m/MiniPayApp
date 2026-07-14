using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Minipay.Application.Commons.Interfaces;
using Minipay.Domain.Payments;

namespace Minipay.Infrastructure
{
    public sealed class EfPaymentRepository :IPaymentRepository
    {
        // construcor injrction so EfPaymentRepository recieves a MiniPayDbContetxt
        private readonly MinipayDbContext _dbContext;

        public EfPaymentRepository(MinipayDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //starts tracking payment object in DbContext "EntityState.Added"/ this entity needs to be inserted, async.. checks if ID avalibale, not needed for guid
        public async Task AddAsync(Payment payment, CancellationToken cancellationToken = default)
        {
            // acutally hits db
            await _dbContext.Payments.AddAsync(payment, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
         => _dbContext.Payments.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        public async Task<IEnumerable<Payment>> GetByStatusAsync(PaymentStatus status, CancellationToken cancellationToken = default)
         => await _dbContext.Payments.Where(p => p.Status == status).ToListAsync(cancellationToken);

        public async Task UpdateAsync(Payment payment, CancellationToken cancellationToken = default)
        {
            _dbContext.Payments.Update(payment);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }


    }
}
