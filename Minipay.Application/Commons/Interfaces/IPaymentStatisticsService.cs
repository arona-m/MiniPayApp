using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minipay.Application.Payments.Dtos;
namespace Minipay.Application.Commons.Interfaces
{
     public interface IPaymentStatisticsService
    {
        void RecordCreated();
        void RecordAuthorized();
        void RecordSettled();
        void RecordFailed();
        PaymentStatisticsDto GetStatistics();
    }
}
