using Banking.Application.Security.Interfaces;
using Banking.Application.Services;
using Banking.Application.Services.Interfaces;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Banking.Application.Extensions
{
    public static class ApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(
                typeof(ApplicationServiceCollectionExtensions)
                .Assembly);

            services.AddScoped<ICustomerService, CustomerService>();

            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<IBankAccountService, BankAccountService>();

            services.AddScoped<ITransactionService, TransactionService>();

            return services;
        }
    }
}
