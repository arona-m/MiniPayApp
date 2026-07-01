using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minipay.Domain.Common
{
    public abstract class AggregateRoot
        //private bc only this class can add events, public readonly app layer needs to dispatch events but cant call add() or clear on whats returned.
    {
        private readonly List<IDomainEvent> _domainEvents = new();

        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        //subclasses can raise events, the rest not.
        protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

        //app layer calls it.
        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}
