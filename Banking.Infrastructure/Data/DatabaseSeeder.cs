using Banking.Application.Security.Interfaces;
using Banking.Domain.Entities;
using Banking.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Banking.Infrastructure.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(
            BankingDbContext context,
            IPasswordHasher passwordHasher)
        {
            await context.Database.MigrateAsync();

            var adminExists = await context.Users
                .AnyAsync(x => x.Role == UserRoleEnum.Admin);

            if (adminExists)
                return;


            var admin = new User(
                "admin",
                passwordHasher.Hash("Admin123!"),
                UserRoleEnum.Admin
            );


            await context.Users.AddAsync(admin);

            await context.SaveChangesAsync();
        }
    }
}