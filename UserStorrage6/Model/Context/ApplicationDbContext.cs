using HotChocolate.Language;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserStorrage6.Model.DB;

namespace UsersStorrage.Models.Context
{
    public class ApplicationDbContext : DbContext
    {
        private readonly string _connectionString;
        private readonly ILoggerFactory _LoggerFactory;

        public DbSet<Service> Services { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<History> Historys { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }

        public ApplicationDbContext(
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            DbContextOptions<ApplicationDbContext> options
            ) : base(options)
        {
#if DEBUG
            _connectionString = configuration.GetValue<string>("USER_DB_DEBUG");
#else
            _connectionString = configuration.GetValue<string>("USER_DB");
#endif
            if (string.IsNullOrEmpty(_connectionString))
                _connectionString = "Host=127.0.0.1;Port=5432;Database=DebugUserStorrage;Username=User;Password=12345678";
            _LoggerFactory = loggerFactory;

            Database.EnsureCreated();
        }

        public ApplicationDbContext(
            IConfiguration configuration,
            ILoggerFactory loggerFactory
            ) : base()
        {
#if DEBUG
            _connectionString = configuration.GetValue<string>("USER_DB_DEBUG");
#else
            _connectionString = configuration.GetValue<string>("USER_DB");
#endif
            if (string.IsNullOrEmpty(_connectionString))
                _connectionString = "Host=127.0.0.1;Port=5432;Database=DebugUserStorrage;Username=User;Password=12345678";
            _LoggerFactory = loggerFactory;

            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasAlternateKey("SysId", "ServiceKey");

            modelBuilder.Entity<Role>()
                .HasAlternateKey("SysId", "ServiceKey");

            modelBuilder.Entity<Permission>()
                .HasAlternateKey("SysId", "ServiceKey");

            modelBuilder.Entity<User>()
                .HasMany(c => c.Roles)
                .WithMany(s => s.Users);

            modelBuilder.Entity<User>()
                .HasMany(c => c.Permissions)
                .WithMany(s => s.Users);

            modelBuilder.Entity<Role>()
                .HasMany(c => c.SysPermitions)
                .WithMany(s => s.Roles);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connectionString);
            optionsBuilder.UseLoggerFactory(_LoggerFactory);
        }
    }
}
