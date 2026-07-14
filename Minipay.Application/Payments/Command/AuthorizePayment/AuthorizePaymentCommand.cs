using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minipay.Application.Payments.Command.AuthorizePayment;

    public sealed record AuthorizePaymentCommand (Guid PaymentId);

