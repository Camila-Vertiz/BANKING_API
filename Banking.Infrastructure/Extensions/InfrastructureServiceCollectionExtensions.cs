using Banking.Domain.Interfaces;
using Banking.Infrastructure.Data;
using Banking.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Banking.Infrastructure.Extensions
{
    public static class InfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure
        (
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            // Configure DbContext
            services.AddDbContext<BankingDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")
                )
            );

            // Register repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IBankAccountRepository, BankAccountRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}