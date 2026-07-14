using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Minipay.Domain.Payments;

namespace Minipay.Infrastructure
{
    public sealed class MinipayDbContext : DbContext
    {
        //constructor, pass options to base class
        public MinipayDbContext(DbContextOptions<MinipayDbContext> options) : base(options)
        {  
        }
        public DbSet<Payment> Payments => Set<Payment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PaymentEntityConfiguration());
        }
    }
}
