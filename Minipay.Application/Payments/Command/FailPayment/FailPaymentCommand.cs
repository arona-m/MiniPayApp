using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minipay.Application.Payments.Command.FailPayment
{
    public sealed record FailPaymentCommand(Guid PaymentId, string Reason);
    
}
