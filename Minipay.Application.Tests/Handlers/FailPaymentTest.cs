using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Minipay.Application.Payments.Command.FailPayment;
using Minipay.Application.Payments.Exceptions;
using Minipay.Application.Tests.FakeRepo;
using Minipay.Domain.Payments;
using Minipay.Domain.ValueObjects;

namespace Minipay.Application.Tests.Handlers
{
    public class FailPaymentTest
    {
        [Fact]

        public async Task HandleAsync_WithCreatedPayment_TransitionToFailed()
        {
            // arrange
            var repository = new FakePaymentRepository();
            var payment = Payment.Create(new Money(100, "EUR"));
            await repository.AddAsync(payment);

            var handler = new FailPaymentHandler(
                repository,
                NullLogger<FailPaymentHandler>.Instance);

            //act
            var result = await handler.HandleAsync(new FailPaymentCommand(payment.Id, "card declined"));

            //assert
            result.Status.Should().Be(nameof(PaymentStatus.Failed));
            result.FailureReason.Should().Be("card declined");
        }
        [Fact]

        public async Task HandleAsync_WithAuthorizedPayment_TransitionToFailed()
        {
            // arrange
            var repository = new FakePaymentRepository();
            var payment = Payment.Create(new Money(100, "EUR"));
            payment.Authorize();

            await repository.AddAsync(payment);
            var handler = new FailPaymentHandler(repository, NullLogger<FailPaymentHandler>.Instance);

            //act
            var result = await handler.HandleAsync(new FailPaymentCommand(payment.Id, "Issuer timeout"));

            //assert
            result.Status.Should().Be(nameof(PaymentStatus.Failed));
            result.FailureReason.Should().Be("Issuer timeout");
        }
        [Fact]
        public async Task HandleAsync_WithSettledPayment_ThrowsInvalidTransition()
        {
            var repository = new FakePaymentRepository();
            var handler = new FailPaymentHandler(repository, NullLogger<FailPaymentHandler>.Instance);

            var act = async () => await handler.HandleAsync(new FailPaymentCommand(Guid.NewGuid(), "some reason"));

            await act.Should().ThrowAsync<PaymentNotFoundException>();

        }

    }
}
