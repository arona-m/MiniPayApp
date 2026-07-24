using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Minipay.Application.Commons.Interfaces;
using Minipay.Application.Payments.Command.FailPayment;
using Minipay.Application.Payments.Exceptions;
using Minipay.Domain.Payments;
using Minipay.Domain.Payments.Exceptions;
using Minipay.Domain.ValueObjects;
using Moq;

namespace Minipay.Application.Tests.Handlers
{
    public class FailPaymentTest
    {
        public readonly Mock<IPaymentRepository> _repositoryMock = new();
        public readonly Mock<IPaymentStatisticsService> _statisticsMock = new();

        private FailPaymentHandler BuildHandler() => new(
            _repositoryMock.Object,
            _statisticsMock.Object,
            NullLogger<FailPaymentHandler>.Instance);

        [Fact]

        public async Task HandleAsync_WithCreatedPayment_TransitionToFailed()
        {
            // arrange
            var payment = Payment.Create(new Money(100, "EUR"));

            _repositoryMock.
                Setup(r => r.GetByIdAsync(payment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(payment);

            var handler = BuildHandler();

            //act
            var result = await handler.HandleAsync(new FailPaymentCommand(payment.Id, "card declined"));

            //assert
            result.Status.Should().Be(nameof(PaymentStatus.Failed));
            result.FailureReason.Should().Be("card declined");

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]

        public async Task HandleAsync_WithAuthorizedPayment_TransitionToFailed()
        {
            //arrange
            var payment = Payment.Create(new Money(100, "EUR"));
            payment.Authorize();

            _repositoryMock.Setup
                (r => r.GetByIdAsync(payment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(payment);

            var handler = BuildHandler();

            //act
            var result = await handler.HandleAsync(new FailPaymentCommand(payment.Id, "issuer time out"));

            //assert
            result.Status.Should().Be(nameof(PaymentStatus.Failed));
            result.FailureReason.Should().Be("issuer time out");

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WithSettledPayment_ThrowsInvalidTransition()
        {
            //arrange
            var payment = Payment.Create(new Money(100, "EUR"));
            payment.Authorize();
            payment.Settle();

            _repositoryMock.Setup
                (r => r.GetByIdAsync(payment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(payment);

            var handler = BuildHandler();

            //act
            var act = async() => await handler.HandleAsync(new FailPaymentCommand(payment.Id, "too late"));

            //assert
            await act.Should().ThrowAsync<InvalidPaymentStateTransitionException>()
                   .Where(e => e.CurrentSatus == PaymentStatus.Settled
                   && e.AttemptedStatus == PaymentStatus.Failed);

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()),Times.Never);

        }

        [Fact]
        public async Task HandleAsync_WithAlreadyFailedPayment_ThrowsInvalidTransition()
        {
            //arrange
            var payment = Payment.Create(new Money(100, "EUR"));
            payment.Fail("first failure");

            _repositoryMock.Setup
                (r => r.GetByIdAsync(payment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(payment);

            var handler = BuildHandler();

            //act
            var act = async () => await handler.HandleAsync(new FailPaymentCommand(payment.Id,"second failure"));

            // assert
            await act.Should().ThrowAsync<InvalidPaymentStateTransitionException>();
             _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Never);
        }
        [Fact]
        public async Task HandleAsync_WithUnknownId_ThrowsPaymentNotFound()
        {
            // arrange
            _repositoryMock.Setup
                (r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Payment?)null);

            var handler = BuildHandler();

            //act
            var act = async () => await handler.HandleAsync(new FailPaymentCommand(Guid.NewGuid(), "some reason"));

            //assert
            await act.Should().ThrowAsync<PaymentNotFoundException>();
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Never);
        }

    }
}
