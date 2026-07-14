using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minipay.Application.Payments.IntegrationEvent
{

    //used to communicate state changes between different microservices, for example, when a payment is authorized, this event can be published to notify other services that the payment has been authorized and they can take appropriate actions.
    public sealed record PaymentAuthorizedIntegrationEvent
    (
        Guid PaymentId,
        decimal Amount,
        string Currency,
        DateTime CreatedAt
    );
}
