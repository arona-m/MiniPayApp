using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Minipay.Application.Commons.Interfaces;
using Minipay.Application.Payments.Command.CreatePayment;
using Minipay.Domain.Payments;
using Moq;
using Xunit;


namespace Minipay.Application.Tests.CreatePaymentHandle
{
    public class CreatePaymentTest
    {
        private readonly Mock<IPaymentRepository> _repositoryMock = new();
        private readonly Mock<ICurrencyValidator> _currencyValidatorMock = new();

        private CreatePaymentHandler BuildHandler() => new(
            _repositoryMock.Object,
            _currencyValidatorMock.Object,
            NullLogger<CreatePaymentHandler>.Instance);

        [Fact]
        public async Task HandleAsync_WithValidInput_CreatePayment()
        {
            // arrange
            _currencyValidatorMock.Setup(v => v.IsSupported("EUR")).Returns(true);

            var handler = BuildHandler();

            //act
            var result = await handler.HandleAsync(new CreatePaymentCommand(100, "EUR"));

            //assert
            result.Amount.Should().Be(100);
            result.Currency.Should().Be("EUR");
            result.Status.Should().Be(nameof(PaymentStatus.Created));

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WithZeroAmount_ThrowsAndNeveCallsRepository()
        {
            //arrange
            _currencyValidatorMock.Setup(v => v.IsSupported(It.IsAny<string>())).Returns(true);
            var handler = BuildHandler();

            //act
            var act = async () => await handler.HandleAsync(
                new CreatePaymentCommand(0, "EUR"));

            //assert
            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Payment>(),It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WithNegativeAmount_ThrowsAndNeverCallsRepository()
        {
            //arrange
            _currencyValidatorMock.Setup(v => v.IsSupported(It.IsAny<string>())).Returns(true);
            var handler = BuildHandler();

            //act
            var act = async () => await handler.HandleAsync(new CreatePaymentCommand(-60, "EUR"));

            //assert
            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WithEmptyCurrency_ThrowsAndNeverCallsRepository()
        {
            //arrange
            _currencyValidatorMock.Setup(v => v.IsSupported("")).Returns(false);
            var handler = BuildHandler();

            //act
            var act = async () => await handler.HandleAsync(new CreatePaymentCommand(100, ""));

            //assert
            await act.Should().ThrowAsync<ArgumentException>();

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Async_WithUnsupportedCurrency_ThrowsAndNeverCallsRepository()
        {
            //arrange
            _currencyValidatorMock.Setup(v => v.IsSupported("XYZ")).Returns(false);
            var handler = BuildHandler();

            //act
            var act = async () => await handler.HandleAsync(new CreatePaymentCommand(100, "XYZ"));

            //assert
            await act.Should().ThrowAsync<ArgumentException>();

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Never);
        }
         
    }
}
