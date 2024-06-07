using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.User;
using Domain.User.ValueObjects;

namespace Infrastructure.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.OwnsOne(u => u.Password, pw =>
                {
                    pw.Property(p => p.HashedPassword).HasColumnName("PasswordHash").UsePropertyAccessMode(PropertyAccessMode.Field);
                });
            });
        }
    }
}
