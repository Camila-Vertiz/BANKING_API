using Banking.Application.Services;
using Banking.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Banking.Application.Extensions
{
    public static class ApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            services.AddScoped<ICustomerService, CustomerService>();

            return services;
        }
    }
}
