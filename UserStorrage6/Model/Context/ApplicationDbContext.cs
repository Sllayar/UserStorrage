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
        private readonly IConfiguration _Configuration;
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
            _Configuration = configuration;
            _LoggerFactory = loggerFactory;

            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connection = _Configuration.GetConnectionString("PostgresDbConnection");

            if (string.IsNullOrEmpty(connection)) 
                connection = "Host=127.0.0.1;Port=5432;Database=UserStorrage;Username=User;Password=1234567890";

            optionsBuilder.UseNpgsql(connection);
            optionsBuilder.UseLoggerFactory(_LoggerFactory);
        }
    }
}
