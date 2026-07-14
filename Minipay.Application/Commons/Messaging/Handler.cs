using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minipay.Application.Commons.Messaging

// CQRRS pattern to separate commands and queries. Commands change state, queries return data. TCommand "CreatePaymentCommand" request to change smth TQuery "GetPaymentById" returns data. TResult is the return type of the command or query.
{
    public interface ICommandHandler<in TCommand, TResult>
    {
        Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
    }
    public interface IQueryHandler<TQuery, TResult> 
    {
        Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
    }
}
