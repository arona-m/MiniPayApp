using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Minipay.Application.Payments.Command.SettlePayment;
using Minipay.Application.Payments.Exceptions;
using Minipay.Application.Tests.FakeRepo;
using Minipay.Domain.Payments;
using Minipay.Domain.Payments.Exceptions;
using Minipay.Domain.ValueObjects;

namespace Minipay.Application.Tests.Handlers
{
    public class SettlePaymentTest
    {
        [Fact]
        public async Task HandleAsync_WithAuthorizedPayment_TransitionToSettled()
        {
            var repository = new FakePaymentRepository();
            var payment = Payment.Create(new Money(100,"EUR"));
            payment.Authorize();
            await repository.AddAsync(payment);

            var handler = new SettlePaymentHandler(repository, NullLogger<SettlePaymentHandler>.Instance);

            var result = await handler.HandleAsync(new SettlePaymentCommand(payment.Id));

            result.Status.Should().Be(nameof(PaymentStatus.Settled));
        }
        [Fact]
        public async Task HandleAsync_WithCreatedPayment_THrowsInvalidTransition()
        {
            var repository = new FakePaymentRepository();
            var payment = Payment.Create(new Money(100, "EUR"));
            await repository.AddAsync(payment);

            var handler = new SettlePaymentHandler(repository, NullLogger<SettlePaymentHandler>.Instance);

            var act = async () => await handler.HandleAsync(new SettlePaymentCommand(payment.Id));

            await act.Should().ThrowAsync<InvalidPaymentStateTransitionException>()
                .Where(e => e.CurrentSatus == PaymentStatus.Created
                       && e.AttemptedStatus == PaymentStatus.Settled);
        }

        [Fact]
        public async Task HandleAsync_WithAlreadySettledPayment_ThrowsInvalidTransition()
        {
            var repository = new FakePaymentRepository();
            var payment = Payment.Create(new Money(100, "EUR"));
            payment.Authorize();
            payment.Settle();
            await repository.AddAsync(payment);

            var handler = new SettlePaymentHandler(repository, NullLogger<SettlePaymentHandler>.Instance);

            var act = async () => await handler.HandleAsync(
                new SettlePaymentCommand(payment.Id));

            await act.Should().ThrowAsync<InvalidPaymentStateTransitionException>();
        }

        [Fact]
        public async Task HandleAsync_WithUnknownId_ThrowsPaymentNotFoundException()
        {
            var repository = new FakePaymentRepository();
            var handler = new SettlePaymentHandler(repository, NullLogger<SettlePaymentHandler>.Instance);

            var act = async () => await handler.HandleAsync(new SettlePaymentCommand(Guid.NewGuid()));

            await act.Should().ThrowAsync<PaymentNotFoundException>();
        }
    }

}
