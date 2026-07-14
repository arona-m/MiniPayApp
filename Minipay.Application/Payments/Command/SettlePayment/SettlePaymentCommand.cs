using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minipay.Application.Payments.Command.SettlePayment
{
    public sealed record SettlePaymentCommand(Guid PaymentId)
    {
    }
}
