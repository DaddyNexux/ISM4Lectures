using FuckingLectures.Models;
using FuckingLectures.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FuckingLectures.Data
{
    public class AppData : IdentityDbContext<User, ApplicationRole, Guid>
    {
        public AppData(DbContextOptions<AppData> options) : base(options) { }


        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        public DbSet<Lectures> Lectures { get; set; }
        public DbSet<Course> Course { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
            .HasIndex(u => u.PhoneNumber)
            .IsUnique();



        }
    }
}
