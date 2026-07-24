using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Minipay.Application.Commons.Interfaces;
using Minipay.Application.Payments.Command.AuthorizePayment;
using Minipay.Application.Payments.Command.CreatePayment;
using Minipay.Application.Payments.Exceptions;
using Minipay.Domain.Payments;
using Minipay.Domain.Payments.Exceptions;
using Minipay.Domain.ValueObjects;
using Moq;
using Xunit;

namespace Minipay.Application.Tests.Handlers
{
    public class AuthorizePaymentTest
    {
        private readonly Mock<IPaymentRepository> _repositoryMock = new();
        private readonly Mock<IPaymentStatisticsService> _statisticsMock = new();

        private AuthorizePaymentHandler BuildHandler() => new(
        _repositoryMock.Object,
        _statisticsMock.Object,
        NullLogger<AuthorizePaymentHandler>.Instance);


        [Fact]
        public async Task HandleAsync_WithExistingCreatedPayment_AuthorizesPayment()
        {
            // arrange 
            var payment = Payment.Create(new Money(100, "EUR"));
            _repositoryMock.Setup(r => r.GetByIdAsync(payment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(payment);

            var handler = BuildHandler();

            // act
            var result = await handler.HandleAsync(new AuthorizePaymentCommand(payment.Id));

            result.Status.Should().Be(nameof(PaymentStatus.Authorized));
            //result.Message.Should().Be("Success: status changed from Created to Authorized");

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WithUnknownPaymentId_ThrowPaymentNotFoundException()
        {
            // arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Payment?)null);

            var handler = BuildHandler();

            //act
            var act = async () => await handler.HandleAsync(new AuthorizePaymentCommand(Guid.NewGuid()));

            //assert
            await act.Should().ThrowAsync<PaymentNotFoundException>();

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Never);
        }
        [Fact]
        public async Task HandleAsync_WithSettledPayment_ThrowsInvalidTransition()
        {
            // arrange
            var payment = Payment.Create(new Money(100, "EUR"));
            payment.Authorize();
            payment.Settle();

            _repositoryMock.Setup(r => r.GetByIdAsync(payment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(payment);

            var handler = BuildHandler();

            //act
            var act = async () => await handler.HandleAsync(new AuthorizePaymentCommand(payment.Id));

            //assert
            await act.Should().ThrowAsync<InvalidPaymentStateTransitionException>();

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WithFailedPayment_ThrowsInvalidTransition()
        {
            var payment = Payment.Create(new Money(100, "EUR"));
            payment.Fail("card declined");

            _repositoryMock.Setup(r => r.GetByIdAsync(payment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(payment);

            var handler = BuildHandler();

            //act
            var act = async () => await handler.HandleAsync(new AuthorizePaymentCommand(payment.Id));

            //assert
            await act.Should().ThrowAsync<InvalidPaymentStateTransitionException>();

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Never);
        }

    }
}
