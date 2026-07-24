Created the Domain layer for MiniPay App.
DDD -> Enitity : Payment is an entity, it has an Id. Two payments with the same Amount but different Id, are different payments.
       Value Object : Money, new Money (100, EUR) and another new Money (100, EUR) are equal in every sense, immutable and self validatimg.
       Aggregate: Cluster of related domain objects (entities and value objects) Payment is the entire aggregate, it owns Money as a sub object
       Aggregate Root : Entity in aggregate through which all changes must flow, nothing external can write to Payment.status directly.
       Domain Event : Immutable record of something inside domain, raised by aggregates
       Always Valid : A domain object should never exist in an invalid state, achieved through constructor validation Money, private setters Payment, and factory methods Payment.Create
SOLID -> Single Responsibility Principle: Each class has a reason to change. Money only changes if money representation rules change, Payment only changes if payment life cycle rules change,
                                           AggregateRoot changes only if the domain event management changes.
       Open/Closed Princip;e: Payment's state is open for extension(new transition row to the switch) and closed for modification (methods that exists dont need to change)
       Liskov Substitution Principle: Payment inherits AggregateRoot and doesnt override its behaviour but adds to it. Whoever holds an AggrgateRoot reference could hold a Paymwent
       Interface Segregation Principle: IDomainEvent is a very small interface that only holds OcurredOnUtc, not a interface that would force implementors to use methods they dont need.
       Dependency Inversion Principle: Minipay.Domain project depends on nothing.
