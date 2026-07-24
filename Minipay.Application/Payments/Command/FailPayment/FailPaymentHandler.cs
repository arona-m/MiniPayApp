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
        private readonly IPaymentStatisticsService _statistics;
        private readonly ILogger <FailPaymentHandler>_logger;


        public FailPaymentHandler(
           IPaymentRepository paymentRepository,
           IPaymentStatisticsService statistics,
           ILogger<FailPaymentHandler> logger)
        {
            _paymentRepository = paymentRepository;
            _statistics = statistics;
            _logger = logger;
        }

        public async Task<PaymentDto> HandleAsync(FailPaymentCommand command, CancellationToken cancellationToken = default)
        {
            var payment = await _paymentRepository.GetByIdAsync(command.PaymentId, cancellationToken)
                ?? throw new PaymentNotFoundException(command.PaymentId);

            payment.Fail(command.Reason);

            await _paymentRepository.UpdateAsync(payment, cancellationToken);
            _statistics.RecordFailed();
            

            _logger.LogInformation("Payment {PaymentId} failed. Reason: {Reason}", payment.Id, command.Reason);

            return PaymentDto.FromDomain(payment);


        }
    }
}
