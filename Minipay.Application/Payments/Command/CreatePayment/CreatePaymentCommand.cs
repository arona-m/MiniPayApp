using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minipay.Application.Payments.Command.CreatePayment
{
    public sealed record CreatePaymentCommand(decimal Amount, string Currency);
    
}
