using Minipay.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Minipay.Domain.Tests.MoneyValueObject
{
    public class MoneyTest
    {
        [Theory]
        [InlineData(100, "EUR")]
        [InlineData(50, "USD")]

        public void Constructor_WithValidInput_ShouldSucceed(decimal amount, string currency)
        {
            var money = new Money(amount, currency);
            money.Currency.Should().Be(currency);
        }

        [Fact]
        public void Constructor_WithNegativeAmount_ShouldThrowArgumentOutOfRangeException()
        {

            var act = () => new Money(-100, "EUR");

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Constructor_WithZeroAmount_ShouldThrowArgumentOutOfRangeException()
        {
            var act = () => new Money(0, "EUR");

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Constructor_WithEmptyCurrency_ShouldThrowArgumentException()
        {
            var act = () => new Money(100, "");

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Constructor_WithNotThreeLetters_ShouldThrowArgumentException()
        {
            var act= () => new Money(100, "EU");

            act.Should().Throw<ArgumentException>();
        }
        [Fact]
        public void Constructor_NormalisesCurrencyToUpperCase()
        {
            var money = new Money(100, "eur");

            money.Currency.Should().Be("EUR");
        }

        [Fact]
        public void Equality_TwoEqualMoneyValues_ShouldBeEqual()
        {
            var a = new Money(100, "EUR");
            var b = new Money(100, "EUR");

            a.Should().Be(b);
            (a == b).Should().BeTrue();
            a.GetHashCode().Should().Be(b.GetHashCode());
        }

        [Fact]
        public void Equality_DifferentAmounts_ShouldNotBeEqual()
        {
            var a = new Money(100, "EUR");
            var b = new Money(50, "EUR");

            a.Should().NotBe(b);
            (a != b).Should().BeTrue();
        }

        [Fact]
        public void Equality_DifferentCorruncies_ShouldNotBeEqual()
        {
            var a = new Money(100, "EUR");
            var b = new Money(100, "USD");

            a.Should().NotBe(b);
        }
        [Fact]
        public void Money_IsImmutable_NoPublicSetterExists()
        {
            var properties = typeof(Money).GetProperties();
            properties.Should().OnlyContain(p => p.SetMethod == null || !p.SetMethod.IsPublic);
        }

    }
}
