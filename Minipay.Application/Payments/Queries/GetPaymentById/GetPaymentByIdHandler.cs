using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minipay.Application.Commons.Interfaces;
using Minipay.Application.Commons.Messaging;
using Minipay.Application.Payments.Dtos;
using Minipay.Application.Payments.Exceptions;

//only reads, loads a payment, throw if misssing return a dto. no update or eventbus for publishing. no logger needed.
namespace Minipay.Application.Payments.Queries.GetPaymentById
{
    public sealed class GetPaymentByIdHandler : IQueryHandler<GetPaymentByIdQuery, PaymentDto>
    {
        private readonly IPaymentRepository _paymentRepository;

        public GetPaymentByIdHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<PaymentDto> HandleAsync(GetPaymentByIdQuery query, CancellationToken cancellationToken = default)
        {
            var payment = await _paymentRepository.GetByIdAsync(query.PaymentId, cancellationToken)
                ?? throw new PaymentNotFoundException(query.PaymentId);

            return PaymentDto.FromDomain(payment);
        }
    }
    
    
}
