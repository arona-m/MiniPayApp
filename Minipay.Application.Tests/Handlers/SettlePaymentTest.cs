using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Minipay.Application.Commons.Interfaces;
using Minipay.Application.Payments.Command.SettlePayment;
using Minipay.Application.Payments.Exceptions;

using Minipay.Domain.Payments;
using Minipay.Domain.Payments.Exceptions;
using Minipay.Domain.ValueObjects;
using Moq;

namespace Minipay.Application.Tests.Handlers
{
    public class SettlePaymentTest
    {
        private readonly Mock<IPaymentRepository> _repositoryMock = new ();
        [Fact]
        public async Task HandleAsync_WithAuthorizedPayment_TransitionToSettled()
        {
            var payment = Payment.Create(new Money(100,"EUR"));
            payment.Authorize();

            _repositoryMock.Setup(r => r.GetByIdAsync(payment.Id,It.IsAny<CancellationToken>())).ReturnsAsync(payment);

            var handler = new SettlePaymentHandler(_repositoryMock.Object, NullLogger<SettlePaymentHandler>.Instance);

            //act
            var result = await handler.HandleAsync(new SettlePaymentCommand(payment.Id));

            //assert
            result.Status.Should().Be(nameof(PaymentStatus.Settled));

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        [Fact]
        public async Task HandleAsync_WithCreatedPayment_THrowsInvalidTransition()
        {
            var payment = Payment.Create(new Money(100, "EUR"));
            _repositoryMock.Setup(r => r.GetByIdAsync(payment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(payment);

            var handler = new SettlePaymentHandler(_repositoryMock.Object, NullLogger<SettlePaymentHandler>.Instance);

            //act
            var act = async() => await handler.HandleAsync(new SettlePaymentCommand(payment.Id));
            //assert
            await act.Should().ThrowAsync<InvalidPaymentStateTransitionException>()
                .Where(e => e.CurrentSatus == PaymentStatus.Created
                && e.AttemptedStatus == PaymentStatus.Settled);

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WithAlreadySettledPayment_ThrowsInvalidTransition()
        {
            //arrange
            var payment = Payment.Create(new Money(100, "EUR"));
            payment.Authorize();
            payment.Settle();

            _repositoryMock.Setup(r => r.GetByIdAsync(payment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(payment);

            var handler = new SettlePaymentHandler(_repositoryMock.Object, NullLogger<SettlePaymentHandler>.Instance);
            //act
            var act = async () => await handler.HandleAsync(new SettlePaymentCommand(payment.Id));

            //assert
            await act.Should().ThrowAsync<InvalidPaymentStateTransitionException>();
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Never);
        }
        [Fact]
        public async Task HandleAsync_WithFailedPayment_ThrowsInvalidTransition()
        {
            //arrange
            var payment = Payment.Create(new Money(100, "EUR"));
            payment.Fail("Card declined");

            _repositoryMock.Setup(r => r.GetByIdAsync(payment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(payment);

            var handler = new SettlePaymentHandler(_repositoryMock.Object, NullLogger<SettlePaymentHandler>.Instance);
            //act
            var act = async () => await handler.HandleAsync(new SettlePaymentCommand(payment.Id));
            //assert

            await act.Should().ThrowAsync<InvalidPaymentStateTransitionException>()
                .Where(e => e.CurrentSatus == PaymentStatus.Failed
                && e.AttemptedStatus == PaymentStatus.Settled);

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Never);
        }
        [Fact]
        public async Task HandleAsync_WithUnknownId_ThrowsPaymentNotFoundException()
        {
            _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Payment?)null);
            var handler = new SettlePaymentHandler(_repositoryMock.Object, NullLogger<SettlePaymentHandler>.Instance);

            //act
            var act = async () => await handler.HandleAsync(new SettlePaymentCommand(Guid.NewGuid()));
            //await
            await act.Should().ThrowAsync<PaymentNotFoundException>();
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Never);

        }
    }

}
