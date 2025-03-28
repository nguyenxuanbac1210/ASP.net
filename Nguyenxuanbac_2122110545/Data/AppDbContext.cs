using Microsoft.EntityFrameworkCore;
using Nguyenxuanbac_2122110545.Model;

namespace Nguyenxuanbac_2122110545.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
    }
}
