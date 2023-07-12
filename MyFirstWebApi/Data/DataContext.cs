using Microsoft.EntityFrameworkCore;
using MyFirstWebApi.Models;

namespace MyFirstWebApi.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext>options): base(options) 
        { 
        
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Country> Countries { get; set; }   
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Dog>  Dogs { get; set; }
        public DbSet<DogOwner> DogOwners { get; set; }
        public DbSet<DogCategory> DogCategories { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Reviewer> Reviewers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DogCategory>()
                .HasKey(pc => new { pc.DogId, pc.CategoryId });
            modelBuilder.Entity<DogCategory>()
                .HasOne(p => p.Dog)
                .WithMany(pc => pc.DogCategories)
                .HasForeignKey(p => p.DogId);
            modelBuilder.Entity<DogCategory>()
                .HasOne(p => p.Category)
                .WithMany(pc => pc.DogCategories)
                .HasForeignKey(p => p.CategoryId);

            modelBuilder.Entity<DogOwner>()
               .HasKey(po => new { po.DogId, po.OwnerId });
            modelBuilder.Entity<DogOwner>()
                .HasOne(p => p.Dog)
                .WithMany(pc => pc.DogOwners)
                .HasForeignKey(p => p.DogId);
            modelBuilder.Entity<DogOwner>()
                .HasOne(p => p.Owner)
                .WithMany(pc => pc.DogOwners)
                .HasForeignKey(p => p.OwnerId);
        }
    }
}
