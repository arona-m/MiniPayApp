using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minipay.Domain.Common;
namespace Minipay.Domain.Payments.Events
{

    // use sealed to prevent inheritance. readonly properties (immutability), facts not commands, self contained data

    //raised when payment is created.
    public sealed class PaymentCreatedDomainEvent : IDomainEvent
    {
        public Guid PaymentId { get; }
        public DateTime OccurredOnUtc { get; }

        public PaymentCreatedDomainEvent(Guid paymentId, DateTime occurredOnUtc)
        {
            PaymentId = paymentId;
            OccurredOnUtc = occurredOnUtc;
        }
    }
    // raised when payment is authorized.
    public sealed class PaymentAuthorizedDomainEvent : IDomainEvent
    {
        public Guid PaymentId { get; }
        public decimal Amount { get; }
        public string Currency { get; }
        public DateTime OccurredOnUtc { get; }

        public PaymentAuthorizedDomainEvent(Guid paymentId, decimal amount, string currency, DateTime occurredOnUtc)
        {
            PaymentId = paymentId;
            Amount = amount;
            Currency = currency;
            OccurredOnUtc = occurredOnUtc;
        }
    }

    // raised when payment is settled.
    public sealed class PaymentSettledDomainEvent : IDomainEvent
    {
        public Guid PaymentId { get; }
        public DateTime OccurredOnUtc { get; }

        public PaymentSettledDomainEvent(Guid paymentId, DateTime occurredOnUtc)
        {
            PaymentId = paymentId;
            OccurredOnUtc = occurredOnUtc;
        }
    }

    // raised when payment fails.
    public sealed class PaymentFailedDomainEvent : IDomainEvent 
    {
        public Guid PaymentId { get; }
        public string Reason { get; }
        public DateTime OccurredOnUtc { get; }

        public PaymentFailedDomainEvent(Guid paymentId, string reason, DateTime occurredOnUtc)
        {
            PaymentId = paymentId;
            Reason = reason;
            OccurredOnUtc = occurredOnUtc;
        }
    }
}
