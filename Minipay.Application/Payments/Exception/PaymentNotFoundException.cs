
namespace Minipay.Application.Payments.Exceptions;

public sealed class PaymentNotFoundException : Exception
{
    public Guid PaymentId { get; }

    public PaymentNotFoundException(Guid paymentId)
        : base($"Payment '{paymentId}' was not found.")
    {
        PaymentId = paymentId;
    }
}