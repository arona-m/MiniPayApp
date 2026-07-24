using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minipay.Application.Commons.Interfaces
{
    public interface ICurrencyValidator
    {
        bool IsSupported(string currency);
    }
}
//validates that a currency is a supported currency.Transient bc a new instance per injection
//in appl bc the handler(appl) depends on it, infras. provides the implementation