using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Minipay.Domain.ValueObjects
{
    public sealed class Money : IEquatable<Money>
    {
        public decimal Amount { get; }

        public string Currency { get; }

        // setting up the business rules.
        public Money(decimal amount, string currency)
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "Money must be greater than zero");
            }
            if (string.IsNullOrWhiteSpace(currency))
            {
                throw new ArgumentException("Currency is required.", nameof(currency));
            }

            var normalisedCurrency = currency.ToUpperInvariant();
            if (normalisedCurrency.Length != 3) 
            {
                throw new ArgumentException("Currency must be 3 letter (ex: EUR,USD).", nameof(currency));
            }
            Amount = amount;
            Currency = normalisedCurrency;
        }

        private void EnsureSameCurrency(Money other)
        {
            if (Currency != other.Currency) 
            {
                throw new InvalidOperationException($"Cant operate on different currencies: {Currency} vs {other.Currency}");
            }
        }
        // compares two Money objects for equality based on their Amount and Currency properties
        public bool Equals(Money? other) 
        {
            if (other is null)
            {
                return false;
            }
            return Amount == other.Amount && Currency == other.Currency;
        }
        //allows comparisons when the object is referenced as a general object type, ensuring that the comparison is still based on the Money properties,
        //if it succeeds Money money, if it doesnt it becomes null, null returns false.
        public override bool Equals(object? obj)
        {
            return Equals(obj as Money);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Amount, Currency);
        }

        public static bool operator == (Money? left, Money? right) => Equals(left, right);
        public static bool operator != (Money? left, Money? right) => !Equals(left, right);

        public override string ToString()
        {
            return $"{Amount} {Currency}";
        }
    }
}
