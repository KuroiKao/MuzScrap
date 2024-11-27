using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuzScrap.BaseContext
{
    public class MuzScrapDbContextFactory : IDesignTimeDbContextFactory<MuzScrapDbContext>
    {
        public MuzScrapDbContext CreateDbContext(string[]? args = null)
        {
            var options = new DbContextOptionsBuilder<MuzScrapDbContext>();
            options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MuzScrapBD;Trusted_Connection=True;");

            return new MuzScrapDbContext(options.Options);
        }
    }
}
