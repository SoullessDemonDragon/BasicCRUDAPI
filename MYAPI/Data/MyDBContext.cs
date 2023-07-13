using Microsoft.EntityFrameworkCore;
using MYAPI.Models;

namespace MYAPI.Data
{
    public class MyDBContext : DbContext
    {
        public MyDBContext(DbContextOptions<MyDBContext>options) : base(options)
        {
            
        }

        public DbSet<Brand> Brands { get; set; }
    }
}
