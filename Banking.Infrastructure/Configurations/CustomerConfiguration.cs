using Banking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Infrastructure.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.DocumentType)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(c => c.DocumentNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(c => new {c.DocumentType, c.DocumentNumber })
                .IsUnique();    

            builder.Property(c => c.FullName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(c => c.Email)
                .IsUnique();

            builder.Property(c => c.UserId)
                .IsRequired(false);

            builder.HasOne<User>()
                .WithOne()
                .HasForeignKey<Customer>(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(c => c.CreatedAtUtc)
                .IsRequired();

            builder.Navigation(c => c.BankAccounts)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
