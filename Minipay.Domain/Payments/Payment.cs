using Minipay.Domain.Common;
using Minipay.Domain.Payments.Events;
using Minipay.Domain.Payments.Exceptions;
using Minipay.Domain.ValueObjects;

namespace Minipay.Domain.Payments
{
    public sealed class Payment : AggregateRoot
    {
        public Guid Id { get; private set; }
        public Money Amount { get; private set; }
        public PaymentStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string? FailureReason { get; private set; }

        private Payment() 
        {
            
        }
        private Payment(Guid id, Money amount, PaymentStatus status, DateTime createdAt) 
        {
            Id = id;
            Amount = amount;
            Status = status;
            CreatedAt = createdAt;
        }

        public static Payment Create(Money amount) 
        {
            var payment = new Payment(Guid.NewGuid(), amount, PaymentStatus.Created, DateTime.UtcNow);
            payment.RaiseDomainEvent(new PaymentCreatedDomainEvent(payment.Id, payment.CreatedAt));
            return payment;
        }

        public void Authorize() 
        {
            EnsureTransitionAllowed(PaymentStatus.Authorized);
            Status = PaymentStatus.Authorized;
            RaiseDomainEvent(new PaymentAuthorizedDomainEvent(Id, Amount.Amount, Amount.Currency, DateTime.UtcNow));
        }

        public void Settle() 
        {
            EnsureTransitionAllowed(PaymentStatus.Settled);
            Status = PaymentStatus.Settled;
            RaiseDomainEvent(new PaymentSettledDomainEvent(Id, DateTime.UtcNow));
        }

        public void Fail(string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                throw new ArgumentException("Failure reason is required.", nameof(reason));
            }
            EnsureTransitionAllowed(PaymentStatus.Failed);
            Status = PaymentStatus.Failed;
            FailureReason = reason;
            RaiseDomainEvent(new PaymentFailedDomainEvent(Id, reason, DateTime.UtcNow));
        }


        // allowed transitions; created -> authorized, craeted-> failed, authorized-> settled, authorized-> failed, others are invalid.
        private void EnsureTransitionAllowed(PaymentStatus target)
        {
            var isAllowed = (Status, target) switch
            {
                (PaymentStatus.Created, PaymentStatus.Authorized) => true,
                (PaymentStatus.Created, PaymentStatus.Failed) => true,
                (PaymentStatus.Authorized, PaymentStatus.Settled) => true,
                (PaymentStatus.Authorized, PaymentStatus.Failed) => true,
                _ => false
            };
            if (!isAllowed)
            {
                throw new InvalidPaymentStateTransitionException(Status, target);
            }
        }
    }
}
