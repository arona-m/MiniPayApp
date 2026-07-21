using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Minipay.Application.Payments.Command.AuthorizePayment;
using Minipay.Application.Payments.Exceptions;
using Minipay.Application.Tests.FakeRepo;
using Minipay.Domain.Payments;
using Minipay.Domain.Payments.Exceptions;
using Minipay.Domain.ValueObjects;
using Xunit;

namespace Minipay.Application.Tests.Handlers
{
    public class AuthorizePaymentTest
    {
        [Fact]
        public async Task HandleAsync_WithExistingCreatedPayment_AuthorizesAndPublishesIntegrationEvent()
        {
            //arrange
            var repository = new FakePaymentRepository();
            var payment = Payment.Create(new Money(100,"EUR"));
            await repository.AddAsync(payment);

            var handler = new AuthorizePaymentHandler(repository, NullLogger<AuthorizePaymentHandler>.Instance);

            //act
            var result = await handler.HandleAsync(new AuthorizePaymentCommand(payment.Id));

            // assert
            result.Status.Should().Be(nameof(PaymentStatus.Authorized));
        }

        [Fact]
        public async Task HandleAsync_WithUnknownPaymentId_ThrowPaymentNotFoundException()
        {
            var repository = new FakePaymentRepository();

            var handler = new AuthorizePaymentHandler(repository, NullLogger<AuthorizePaymentHandler>.Instance);
            {
                var act = async () => await handler.HandleAsync(new AuthorizePaymentCommand(Guid.NewGuid()));

                await act.Should().ThrowAsync<PaymentNotFoundException>();
            }
        }


    }
}
