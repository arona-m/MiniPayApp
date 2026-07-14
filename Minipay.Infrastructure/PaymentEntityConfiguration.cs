using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minipay.Domain.Payments;

namespace Minipay.Infrastructure
{
    // fluent API Mapping 
    public sealed class PaymentEntityConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");
            //id as primary key
            builder.HasKey(p => p.Id);
            //enum to string, if not sql: status:1 
            builder.Property(p => p.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(p => p.CreatedAt).IsRequired();
            builder.Property(p => p.FailureReason).HasMaxLength(500);


            //money is a value object so instead of seperate tables Payment.Amount is an owned entity, lifecycle controlled by Payment
            builder.OwnsOne(p => p.Amount, money =>
            {
                //EF would name this Amount_Amount (property name "Amount" with property name on Money "Amount" HasColumnName overrides that)
                money.Property(m => m.Amount).HasColumnName("Amount").HasColumnType("decimal(18,2)");
                money.Property(m => m.Currency).HasColumnName("Currency").HasMaxLength(3).IsRequired();
            });

            builder.Ignore(p => p.DomainEvents);
        }
    }
}
