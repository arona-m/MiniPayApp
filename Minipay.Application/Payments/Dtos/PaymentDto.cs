using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minipay.Domain.Payments;

namespace Minipay.Application.Payments.Dtos;
     public sealed record PaymentDto
    (
        Guid Id,
        decimal Amount,
        string Currency,
        string Status,
        DateTime CreatedAt,
        string? FailureReason,
        string? Message
        )
{
  public static PaymentDto FromDomain(Payment payment, string? message = null) => new(
     payment.Id,
     // 1 amount => Money value object on Payment, 2 amount => numeric value inside Money dto flattents these two into one
     payment.Amount.Amount,
     payment.Amount.Currency,
     payment.Status.ToString(),
     payment.CreatedAt,
     payment.FailureReason,
     message);
   
}