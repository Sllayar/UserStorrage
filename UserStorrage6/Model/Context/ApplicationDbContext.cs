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
            _connectionString = configuration.GetValue<string>("USER_DB");
            if (string.IsNullOrEmpty(_connectionString))
                throw new Exception("Envariment USER_DB is null");
            _LoggerFactory = loggerFactory;

            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connectionString);
            optionsBuilder.UseLoggerFactory(_LoggerFactory);
        }
    }
}
