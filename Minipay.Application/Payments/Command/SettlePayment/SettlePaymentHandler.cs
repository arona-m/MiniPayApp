using Microsoft.Extensions.Logging;
using Minipay.Application.Commons.Interfaces;
using Minipay.Application.Commons.Messaging;
using Minipay.Application.Payments.Dtos;
using Minipay.Application.Payments.Exceptions;
namespace Minipay.Application.Payments.Command.SettlePayment
{
    public sealed class SettlePaymentHandler : ICommandHandler<SettlePaymentCommand, PaymentDto>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<SettlePaymentHandler> _logger;

        public SettlePaymentHandler(
            IPaymentRepository paymentRepository,
            ILogger<SettlePaymentHandler> logger)
        {
            _paymentRepository = paymentRepository;
            _logger = logger;
        }

        public async Task<PaymentDto> HandleAsync(SettlePaymentCommand command, CancellationToken cancellationToken = default)
        {
            var payment = await _paymentRepository.GetByIdAsync(command.PaymentId, cancellationToken)
                ?? throw new PaymentNotFoundException(command.PaymentId);

            payment.Settle();

            await _paymentRepository.UpdateAsync(payment, cancellationToken);
            _logger.LogInformation("Payment {PaymentId} settled", payment.Id);

            payment.ClearDomainEvents();

            return PaymentDto.FromDomain(payment);
        }
    }
}
