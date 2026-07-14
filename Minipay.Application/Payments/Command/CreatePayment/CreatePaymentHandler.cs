using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Minipay.Application.Commons.Interfaces;
using Minipay.Application.Commons.Messaging;
using Minipay.Application.Payments.Dtos;
using Minipay.Domain.Payments;
using Minipay.Domain.ValueObjects;

namespace Minipay.Application.Payments.Command.CreatePayment
{
    public sealed class CreatePaymentHandler
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<CreatePaymentHandler> _logger;

        //logger is used to log information about the payment creation process. It helps in tracking the flow of the application and debugging issues if they arise.
        public CreatePaymentHandler(IPaymentRepository paymentRepository, ILogger<CreatePaymentHandler> logger)
        {
            _paymentRepository = paymentRepository;
            _logger = logger;
        }

        public async Task<PaymentDto> HandleAsync(CreatePaymentCommand command, CancellationToken cancellationToken = default)
        {
            //Money's constructor enforces "Amount > 0" and "Currency required".
            var Money = new Money(command.Amount, command.Currency);
            var payment = Payment.Create(Money);
            await _paymentRepository.AddAsync(payment, cancellationToken);

            _logger.LogInformation(
             "Payment {PaymentId}  created for {Amount} {Currency}",
             payment.Id, payment.Amount.Amount, payment.Amount.Currency
                );

            return PaymentDto.FromDomain(payment);
        }
    }
}
