using IdentityManagement.API.Core.Models;
using IdentityManagement.API.Core.Security;
using Microsoft.EntityFrameworkCore;

namespace IdentityManagement.API.Persistence
{
    public class AppDbContext : DbContext
    {
        private readonly IPasswordService _passwordSvc;
        public AppDbContext(DbContextOptions opt, IPasswordService passwordSvc) :base(opt)
        {
            _passwordSvc = passwordSvc;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<State> States { get; set; }

        public DbSet<Lga> Lgas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<User>().HasIndex(x => x.Username).IsUnique();

            base.OnModelCreating(modelBuilder);


            SeedDatabase(modelBuilder);
        }

        private void SeedDatabase(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Role>().HasData(new List<Role>
            {
                new Role
                {
                    Id = 1,
                    Name = "Tester"
                },
                new Role
                {
                    Id = 2,
                    Name = "Administrator"
                }
            });

            modelBuilder.Entity<User>().HasData(new List<User>
            {
                new User
                {
                    Id = 1,
                    FirstName = "Joe",
                    LastName = "Bob",
                    Username = "jbob@test.com",
                    PasswordHash = _passwordSvc.HashPassword("password"),
                    PhoneNumber = "08188198932",
                    LgaId = 1,
                    SecurityStamp = "53bed3d7-4290-404f-bb7d-1ecc9ad99b45"
                },
                new User
                {
                    Id = 2,
                    FirstName = "Sam",
                    LastName = "Tom",
                    Username = "stom@test.com",
                    PasswordHash = _passwordSvc.HashPassword("password"),
                    PhoneNumber = "08033083101",
                    LgaId = 2,
                    SecurityStamp = "53bed3d7-4290-404f-bb7d-1ecc9ad99b46"
                }
            });

            modelBuilder.Entity<UserRole>().HasData(new List<UserRole>
            {
                new UserRole{ UserId = 1, RoleId = 1 },
                new UserRole{ UserId = 1, RoleId = 2 },
                new UserRole{ UserId = 2, RoleId = 1 },
            });

            modelBuilder.Entity<State>().HasData(new List<State> 
            { 
                new State
                {
                    Id = 1,
                    Name = "Lagos"
                },
                new State
                {
                    Id = 2,
                    Name = "Abuja"
                }
            });

            modelBuilder.Entity<Lga>().HasData(new List<Lga>
            {
                new Lga
                {
                    Id = 1,
                    Name = "Shomolu",
                    StateId = 1
                },
                new Lga
                {
                    Id = 2,
                    Name = "Makoko",
                    StateId = 1
                },
                new Lga
                {
                    Id = 3,
                    Name = "Gwagwalada",
                    StateId = 2
                },
                new Lga
                {
                    Id = 4,
                    Name = "Wuse",
                    StateId = 2
                }
            });
        }
    }
}
