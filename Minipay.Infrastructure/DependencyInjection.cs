using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Minipay.Application.Commons.Interfaces;
using Minipay.Infrastructure.Validation;

namespace Minipay.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddService(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<MinipayDbContext>(options => options.UseSqlServer(connectionString));

            services.AddScoped<IPaymentRepository, EfPaymentRepository>();

            //new insrance per request, CurrencyV is stateless and has no fields that change 
            services.AddTransient<ICurrencyValidator, CurrencyValidator>();

            return services;
        }
    }
}
