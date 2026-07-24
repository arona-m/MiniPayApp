using Microsoft.Extensions.DependencyInjection;
using Minipay.Application.Commons.Interfaces;
using Minipay.Application.Payments.Command.AuthorizePayment;
using Minipay.Application.Payments.Command.CreatePayment;
using Minipay.Application.Payments.Command.FailPayment;
using Minipay.Application.Payments.Command.SettlePayment;
using Minipay.Application.Payments.Queries.GetPaymentById;
using Minipay.Application.Payments.Queries.GetPaymentByStatus;

namespace Minipay.Application;


public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreatePaymentHandler>();
        services.AddScoped<AuthorizePaymentHandler>();
        services.AddScoped<GetPaymentByIdHandler>();
        services.AddScoped<GetPaymentByStatusHandler>();
        services.AddScoped<FailPaymentHandler>();
        services.AddScoped<SettlePaymentHandler>();
        return services;
    }
}