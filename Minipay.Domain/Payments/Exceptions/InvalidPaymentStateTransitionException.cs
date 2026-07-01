using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minipay.Domain.Payments.Exceptions
{
    //exception class instead of InvalidOperationException to be more specific and allow catching this specific exception type.
    //Reports a domain rule violation when an invalid state transition is attempted on a payment.
    public sealed class InvalidPaymentStateTransitionException : Exception
    {
        public PaymentStatus CurrentSatus { get; }
        public PaymentStatus AttemptedStatus { get; }

        public InvalidPaymentStateTransitionException(PaymentStatus currentStatus, PaymentStatus attemptedStatus)
            : base($"Invalid payment state transition from {currentStatus} to {attemptedStatus}.")
        {
            CurrentSatus = currentStatus;
            AttemptedStatus = attemptedStatus;
        }
    }
}
