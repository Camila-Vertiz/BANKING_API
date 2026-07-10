using Banking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Infrastructure.Configurations
{
    public class BankAccountConfiguration:IEntityTypeConfiguration<BankAccount>
    {
        public void Configure(EntityTypeBuilder<BankAccount> builder)
        {
            builder.ToTable("BankAccounts");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Number)
                .IsRequired()
                .HasMaxLength(30);

            builder.HasIndex(b => b.Number)
                .IsUnique();

            builder.Property(b => b.Balance)
                .IsRequired()
                .HasPrecision(18,2);

            builder.Property(b => b.Currency)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(b => b.CustomerId)
                .IsRequired();

            builder.HasOne<Customer>()
                .WithMany(b => b.BankAccounts)
                .HasForeignKey(b => b.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(b=> b.CreatedAtUtc) 
                .IsRequired();

            builder.Navigation(b => b.Transactions)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
