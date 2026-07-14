using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minipay.Domain.Payments;

namespace Minipay.Application.Payments.Queries.GetPaymentByStatus;
    public sealed record GetPaymentByStatusQuery(PaymentStatus Status);
   

