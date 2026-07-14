using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minipay.Application.Payments.Queries.GetPaymentById
{
    public sealed record GetPaymentByIdQuery(Guid PaymentId)
    {
    }
}
