using Microsoft.EntityFrameworkCore;
using Habr.DataAccess.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Habr.DataAccess.Configurations;

namespace Habr.DataAccess
{
    public class DataContext : DbContext
    {
        public static readonly ILoggerFactory ConsoleLoggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        private readonly string connectionString;

        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Comment> Comments { get; set; }

        /// <summary>
        /// Initializes an instance of a class with the given connection string, for the main operation of the application.
        /// </summary>
        /// <param name="connectionString">The connection string to the database.</param>
        /// <exception cref="ArgumentNullException">Thrown when the connection string is null.</exception>
        public DataContext(string connectionString)
        {
            this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        /// <summary>
        /// Initializes a new instance of the DataContext class using the connection string from the configuration file for manual database migration.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public DataContext()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            this.connectionString = config.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is empty.");
        }

        /// <summary>
        /// Initializes a new instance of the DataContext class using the provided parameters, for use with Moq
        /// </summary>
        /// <param name="options">Options for this context.</param>
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(this.connectionString);
                optionsBuilder.UseLoggerFactory(ConsoleLoggerFactory);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PostConfiguration).Assembly);
        }
    }
}
