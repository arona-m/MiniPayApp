
using Minipay.Domain.Payments;
using Minipay.Domain.Payments.Events;
using Minipay.Domain.Payments.Exceptions;
using Minipay.Domain.ValueObjects;
using Xunit;
using FluentAssertions;

namespace Minipay.Domain.Tests.Payments

{

    // covers lifecycle rules defined in Payment.EnsureTransitionAllowed the valid ones and a set of invalid ones
    public class PaymentTest
    {
        //helper method, instaed of Payment.Create(new Money(100,EUR)) in every test, u only call CreatePayment() with no parameters and get 100 EUR (others: change it)
        private static Payment CreatePayment(decimal amount = 100, string currency = "EUR") =>
            Payment.Create(new Money(amount, currency));


        [Fact]
        public void Create_ShouldStartInCreatedStatus()
        {
            var payment = CreatePayment();

            payment.Status.Should().Be(PaymentStatus.Created);
            payment.Id.Should().NotBe(Guid.Empty);
            payment.FailureReason.Should().BeNull();
        }

        [Fact]
        // exactly one item in DomainEvents matches the condition, efilter that checks the type 
        public void Create_ShouldRaisePaymentCreatedEvent()
        {
            var payment = CreatePayment();

            payment.DomainEvents.Should().ContainSingle(e => e is PaymentCreatedDomainEvent);
        }

        // Valid transitions

        [Fact]
        public void Authorize_FromCreated_ShouldTransitionToAuthorized()
        {
            var payment = CreatePayment(100, "EUR");
            payment.Authorize();

            payment.DomainEvents.OfType<PaymentAuthorizedDomainEvent>().Should().ContainSingle()
            .Which.Should().Match<PaymentAuthorizedDomainEvent>(e =>
            e.PaymentId == payment.Id && e.Amount == 100 && e.Currency == "EUR");
        }

        [Fact]
        public void Settle_FromAuthorized_ShouldTransitionToSettled()
        {
            var payment = CreatePayment();
            payment.Authorize();
            payment.Settle();

            payment.Status.Should().Be(PaymentStatus.Settled);
        }

        [Fact]
        public void Fail_FromCreated_ShouldTransitionToFailedAndRecordReason()
        {
            var payment = CreatePayment();

            payment.Fail("Card declined");

            payment.Status.Should().Be(PaymentStatus.Failed);
        }

        [Fact]
        public void Fail_FromAuthorized_ShouldTransitionToFailed()
        {
            var payment = CreatePayment();
            payment.Authorize();

            payment.Fail("issuer timeout");
            payment.Status.Should().Be(PaymentStatus.Failed);
        }

        // invalid transitions

        [Fact]
        public void Settle_FromCreated_ShouldThrowInvalidPaymentStateTransition()
        {
            // arrange object to test
            var payment = CreatePayment();
            // call the method ur testing
            var act = () => payment.Settle();

            // assert. verify the outcome
            act.Should().Throw<InvalidPaymentStateTransitionException>()
                .Where(e => e.CurrentSatus == PaymentStatus.Created && e.AttemptedStatus == PaymentStatus.Settled);
        }

        [Fact]
        public void Authorize_FromFailed_ShouldThrow()
        {
            var payment = CreatePayment();
            payment.Fail("card declined");

            var act = () => payment.Authorize();
            act.Should().Throw<InvalidPaymentStateTransitionException>()
                .Where(e => e.CurrentSatus == PaymentStatus.Failed && e.AttemptedStatus == PaymentStatus.Authorized);
        }

        [Fact]
        public void Authorize_FromSailed_ShouldThrow()
        {
            var payment = CreatePayment();
            payment.Authorize();
            payment.Settle();

            var act = () => payment.Authorize();

            act.Should().Throw<InvalidPaymentStateTransitionException>()
                .Where(e => e.CurrentSatus == PaymentStatus.Settled && e.AttemptedStatus == PaymentStatus.Authorized);
        }

        [Fact]
        public void Fail_FromSettled_ShouldThrow()
        {
            var payment = CreatePayment();
            payment.Authorize();
            payment.Settle();

            var act = () => payment.Fail("too late");
            act.Should().Throw<InvalidPaymentStateTransitionException>()
               .Where(e => e.CurrentSatus == PaymentStatus.Settled && e.AttemptedStatus == PaymentStatus.Failed);
        }

        [Fact]
        public void Fail_WithEmptyReason_ShouldThrowArgumentException()
        {
            var payment = CreatePayment();
            var act = () => payment.Fail(string.Empty);

            act.Should().Throw<ArgumentException>();
        }


    }
}
