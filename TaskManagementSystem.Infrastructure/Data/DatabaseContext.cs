using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TaskManagementSystem.Core.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace TaskManagementSystem.Infrastructure.Data
{
    public class DatabaseContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public DatabaseContext(DbContextOptions<DatabaseContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Task> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure enums as strings
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();

            modelBuilder.Entity<Task>()
                .Property(t => t.Status)
                .HasConversion<string>();

            // Configure relationships
            modelBuilder.Entity<Task>()
                .HasOne(t => t.AssignedToUser)
                .WithMany(u => u.AssignedTasks)
                .HasForeignKey(t => t.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Task>()
                .HasOne(t => t.CreatedByUser)
                .WithMany(u => u.CreatedTasks)
                .HasForeignKey(t => t.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Task>()
                .HasOne(t => t.Team)
                .WithMany(tm => tm.Tasks)
                .HasForeignKey(t => t.TeamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Team>()
                .HasOne(t => t.CreatedByUser)
                .WithMany()
                .HasForeignKey(t => t.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public async Task SeedDataAsync()
        {
            // Seed default users if they don't exist
            if (!await Users.AnyAsync(u => u.Email == "admin@demo.com"))
            {
                var hasher = new PasswordHasher();
                Users.Add(new User
                {
                    FullName = "System Administrator",
                    Email = "admin@demo.com",
                    PasswordHash = hasher.HashPassword("Admin1231"),
                    Role = UserRole.Admin,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                });
            }

            if (!await Users.AnyAsync(u => u.Email == "manager@demo.com"))
            {
                var hasher = new PasswordHasher();
                Users.Add(new User
                {
                    FullName = "Demo Manager",
                    Email = "manager@demo.com",
                    PasswordHash = hasher.HashPassword("Manager1231"),
                    Role = UserRole.Manager,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                });
            }

            if (!await Users.AnyAsync(u => u.Email == "employee@demo.com"))
            {
                var hasher = new PasswordHasher();
                Users.Add(new User
                {
                    FullName = "Demo Employee",
                    Email = "employee@demo.com",
                    PasswordHash = hasher.HashPassword("Employee1231"),
                    Role = UserRole.Employee,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                });
            }

            await SaveChangesAsync();
        }
    }

    public class PasswordHasher
    {
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
