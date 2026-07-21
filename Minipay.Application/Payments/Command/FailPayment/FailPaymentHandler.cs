using Microsoft.Extensions.Logging;
using Minipay.Application.Commons.Interfaces;
using Minipay.Application.Commons.Messaging;
using Minipay.Application.Payments.Dtos;
using Minipay.Application.Payments.Exceptions;


namespace Minipay.Application.Payments.Command.FailPayment
{
    public sealed class FailPaymentHandler
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger <FailPaymentHandler>_logger;

        public FailPaymentHandler(
           IPaymentRepository paymentRepository,
           ILogger<FailPaymentHandler> logger)
        {
            _paymentRepository = paymentRepository;
            _logger = logger;
        }

        public async Task<PaymentDto> HandleAsync(FailPaymentCommand command, CancellationToken cancellationToken = default)
        {
            var payment = await _paymentRepository.GetByIdAsync(command.PaymentId, cancellationToken)
                ?? throw new PaymentNotFoundException(command.PaymentId);

            payment.Fail(command.Reason);

            await _paymentRepository.UpdateAsync(payment, cancellationToken);

            _logger.LogInformation("Payment {PaymentId} failed. Reason: {Reason}", payment.Id, command.Reason);

            return PaymentDto.FromDomain(payment);


        }
    }
}
