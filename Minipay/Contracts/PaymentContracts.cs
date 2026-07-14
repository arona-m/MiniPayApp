using Minipay.Application.Payments.Dtos;

namespace Minipay.Api.Contracts
{
    public sealed record CreatePaymentRequest(decimal Amount, string Currency);
    
    public sealed record CreatePaymentResponse(Guid Id);

    public sealed record PaymentResponse
    (
        Guid Id,
        decimal Amount,
        string Currency,
        string Status,
        DateTime CreatedAt,
        string? FailureReason)
    {
        public static PaymentResponse FromDto(PaymentDto dto) =>
            new(dto.Id, dto.Amount,dto.Currency, dto.Status, dto.CreatedAt, dto.FailureReason);
    }
    
    public sealed record FailPaymentRequest(string Reason);
}
