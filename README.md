# MiniPay

A payment processing API built to demonstrate **Clean Architecture**, **Domain-Driven Design**, and **SOLID principles** 

---

## Architecture

MiniPay follows Clean Architecture. Dependencies only point inward — outer layers know about inner layers, never the reverse.

```
┌─────────────────────────────┐
│          Minipay.Api        │  HTTP only. No business logic.
├─────────────────────────────┤
│     Minipay.Application     │  Use cases. Orchestrates domain objects.
├─────────────────────────────┤
│       Minipay.Domain        │  Business rules. No external dependencies.
├─────────────────────────────┤
│    Minipay.Infrastructure   │  EF Core + SQL Server. Implements interfaces.
└─────────────────────────────┘
```

---

## Domain Layer

The heart of the application. Contains all business rules and enforces them internally.

**Payment** is the aggregate root. All state changes go through its own methods — no public setters, no external mutation.

```csharp
payment.Authorize();   // enforces Created → Authorized rule internally
payment.Settle();      // enforces Authorized → Settled rule internally
payment.Fail(reason);  // enforces valid transitions internally
```

**Money** is a Value Object. Immutable, self-validating, compared by value not identity.

```csharp
var money = new Money(100, "EUR");  // validates itself on construction
```

**Domain Events** are raised on every state change. The aggregate communicates what happened without knowing what will react to it.

---

## Application Layer

Thin use case handlers that coordinate domain objects, the repository, and logging. No business logic lives here.

Uses **CQRS** — Commands change state, Queries only read it:

| Type | Examples |
|---|---|
| Commands | `CreatePayment`, `AuthorizePayment`, `SettlePayment`, `FailPayment` |
| Queries | `GetPaymentById`, `GetPaymentsByStatus` |

`IPaymentRepository` is declared here, not in Infrastructure. This is **Dependency Inversion** — Application defines what it needs, Infrastructure satisfies it.

---

## Payment Lifecycle

```
            ┌─────────┐
  start ──► │ Created │
            └────┬────┘
             ┌───┴───┐
             ▼       ▼
      ┌────────────┐ ┌────────┐
      │ Authorized │ │ Failed │
      └─────┬──────┘ └────────┘
        ┌───┴───┐
        ▼       ▼
    ┌────────┐ ┌────────┐
    │Settled │ │ Failed │
    └────────┘ └────────┘
```

Rules are enforced by `Payment.EnsureTransitionAllowed()` — one method, one place, no duplication.

---

## Infrastructure Layer

Implements what Application declared. Uses **EF Core 8** with SQL Server.

`Money` is mapped as an **owned type** — stored as columns on the Payments table, not a separate table. This reflects its nature as a Value Object.

```csharp
builder.OwnsOne(p => p.Amount, money => {
    money.Property(m => m.Amount).HasColumnName("Amount");
    money.Property(m => m.Currency).HasColumnName("Currency");
});
```

---

## Testing

Tests use **xUnit** and **FluentAssertions**. Application tests use a `FakePaymentRepository` — no database required.

```bash
dotnet test
```

---

## Tech Stack

`.NET 8` · `EF Core 8` · `SQL Server` · `xUnit` · `FluentAssertions` · `Swagger`
