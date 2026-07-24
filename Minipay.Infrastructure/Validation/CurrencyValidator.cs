using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minipay.Application.Commons.Interfaces;

namespace Minipay.Infrastructure.Validation
{
   public sealed class CurrencyValidator : ICurrencyValidator
    {
        private static readonly HashSet<string> _supported = new()
        {
            "EUR", "USD", "GBP", "ARS","AUD",
            "BRL", "CAD", "CHF", "CNY", "CZK"
        };

        public bool IsSupported(string currency)
        {
            if (string.IsNullOrWhiteSpace(currency)) 
            {
                throw new InvalidOperationException("Currency must be ISO 4217 compliant, no currency provided");
            }

            return _supported.Contains(currency.Trim().ToUpperInvariant()); 
        }

    }
}
// hashset => o(1), list => o(n)