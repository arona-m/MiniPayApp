using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minipay.Application.Commons.Interfaces;
using Minipay.Application.Commons.Messaging;
using Minipay.Application.Payments.Dtos;

namespace Minipay.Application.Payments.Queries.GetPaymentByStatus
{
    public sealed class GetPaymentByStatusHandler : IQueryHandler<GetPaymentByStatusQuery, IReadOnlyCollection<PaymentDto>>
    {
        private readonly IPaymentRepository _paymentRepository;

        public GetPaymentByStatusHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<IReadOnlyCollection<PaymentDto>> HandleAsync(
           GetPaymentByStatusQuery query, CancellationToken cancellationToken = default)
        {
            var payments = await _paymentRepository.GetByStatusAsync(query.Status, cancellationToken);
            return payments.Select(p => PaymentDto.FromDomain(p,null)).ToList();
        }
         
    }
}
