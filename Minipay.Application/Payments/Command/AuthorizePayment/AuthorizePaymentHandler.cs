using Microsoft.Extensions.Logging;
using Minipay.Application.Commons.Interfaces;
using Minipay.Application.Commons.Messaging;
using Minipay.Application.Payments.Dtos;
using Minipay.Application.Payments.Exceptions;
using Minipay.Application.Payments.IntegrationEvent;

namespace Minipay.Application.Payments.Command.AuthorizePayment;

public sealed class AuthorizePaymentHandler : ICommandHandler<AuthorizePaymentCommand, PaymentDto>
{
    private readonly IPaymentRepository _paymentRepository;
    //private readonly IEventBus _eventBus;
    private readonly ILogger<AuthorizePaymentHandler> _logger;

    public AuthorizePaymentHandler(
     IPaymentRepository paymentRepository,
    //IEventBus eventBus,
     ILogger<AuthorizePaymentHandler> logger)
    {
        _paymentRepository = paymentRepository;
        //_eventBus = eventBus;
        _logger = logger;
    }

    public async Task<PaymentDto> HandleAsync(AuthorizePaymentCommand command, CancellationToken cancellationToken = default)
    {
        // get the payment, if null throw PaymentNotFoundException
        var payment = await _paymentRepository.GetByIdAsync(command.PaymentId, cancellationToken)
         ?? throw new PaymentNotFoundException(command.PaymentId);

        //checks EnsureTransitionAllowed, sets status = authorized and raises PaymentAuthorizedDomainEvent. if not allowed, throws InvalidPaymentStateTransitionException
        payment.Authorize();

        await _paymentRepository.UpdateAsync(payment, cancellationToken);
        _logger.LogInformation("Payment {PaymentId} authorized", payment.Id);

        //await _eventBus.PublishAsync(
        //    new PaymentAuthorizedIntegrationEvent(
        //    payment.Id,
        //    payment.Amount.Amount,
        //    payment.Amount.Currency,
        //    DateTime.UtcNow),
        //    cancellationToken); 

        payment.ClearDomainEvents();

        return PaymentDto.FromDomain(payment);
    }
}

// example of what to not do (Bypasses EnsureTransitionAllowed and RaiseDomainEvent) and duplicates transition logic in application where it doesnt belong.
//#if (payment.Status == PaymentStatus.Created)
//{
//   payment.Status = PaymentStatus.Authorized;
//}
//else 
//{
//   throw new Exception("Invalid state transition");
//}