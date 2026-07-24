using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minipay.Application.Payments.Dtos
{
    public sealed record PaymentStatisticsDto(
        int TotalCreated,
        int TotalAuthorized,
        int TotalSettled,
        int TotalFailed,
        int TotalProcessed);
}
