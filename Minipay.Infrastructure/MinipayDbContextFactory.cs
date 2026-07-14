using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Minipay.Infrastructure
{
    public sealed class MinipayDbContextFactory : IDesignTimeDbContextFactory<MinipayDbContext>
    {
        public MinipayDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<MinipayDbContext>()
                .UseSqlServer(
                "Server=.\\SQLEXPRESS;Database=MiniPay;Trusted_Connection=True;TrustServerCertificate=True;")
                .Options;

            return new MinipayDbContext(options);
        }
    }
    
}
