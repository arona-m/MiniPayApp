using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Minipay.Application.Payments.Command.CreatePayment;
using Minipay.Application.Tests.FakeRepo;
using Minipay.Domain.Payments;
using Xunit;


namespace Minipay.Application.Tests.CreatePaymentHandle
{
    public class CreatePaymentTest
    {
        [Fact]
        public async Task HandleAsync_WithValidInput_CreatePayment()
        {
            var repository = new FakePaymentRepository();
            var handler = new CreatePaymentHandler(repository, NullLogger<CreatePaymentHandler>.Instance);

            var result = await handler.HandleAsync(new CreatePaymentCommand(100, "EUR"));

            result.Amount.Should().Be(100);
            result.Currency.Should().Be("EUR");
            result.Status.Should().Be(nameof(PaymentStatus.Created));
            repository.Payments.Should().ContainSingle(p => p.Id == result.Id);
        }

        [Fact]

        public async Task HandleAsync_WithZeroAmount_ThrowsArgumentOutOfRangeException()
        {
            var repository = new FakePaymentRepository();
            var handler = new CreatePaymentHandler(repository, NullLogger<CreatePaymentHandler>.Instance);

            var act = async () => await handler.HandleAsync(new CreatePaymentCommand(0, "EUR"));

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
            repository.Payments.Should().BeEmpty();
        }
         
    }
}
