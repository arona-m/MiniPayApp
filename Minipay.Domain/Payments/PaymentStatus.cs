using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minipay.Domain.Payments
{
    public enum PaymentStatus
    {
        Created = 0,
        Authorized = 1,
        Settled = 2,
        Failed = 3
    }
}
