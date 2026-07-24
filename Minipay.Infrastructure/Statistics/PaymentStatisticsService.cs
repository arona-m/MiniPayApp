using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minipay.Application.Commons.Interfaces;
using Minipay.Application.Payments.Dtos;

namespace Minipay.Infrastructure.Statistics
{
    public sealed class PaymentStatisticsService : IPaymentStatisticsService
    {
        private int _totalCreated;
        private int _totalAuthorized;
        private int _totalSettled;
        private int _totalFailed;

        //interlocked.increment=> safe for concurrent access from multiple threads, ++ wouldnt be reliable, two threads can interleave the read,increment,write steps and corrupt the value
        public void RecordCreated() => Interlocked.Increment(ref _totalCreated);
        public void RecordAuthorized() => Interlocked.Increment(ref _totalAuthorized);
        public void RecordSettled() => Interlocked.Increment(ref _totalSettled);
        public void RecordFailed() => Interlocked.Increment(ref _totalFailed);

        public PaymentStatisticsDto GetStatistics() => new(
            _totalCreated,
            _totalAuthorized,
            _totalSettled,
            _totalFailed,
            TotalProcessed: _totalCreated + _totalAuthorized + _totalSettled + _totalFailed);
    }
}
