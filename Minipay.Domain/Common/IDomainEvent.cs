using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minipay.Domain.Common

    // "this state change happened" only maintains the state change every event has in common, everything else added by concrete event class.
    // AggregateRoot's List<IDomainEvent> would hold no type, alternatives: List<Object> losing type info or duplicating timestamp for every event type.
{
    public interface IDomainEvent
    {
        DateTime OccurredOnUtc { get; }
    }
}
