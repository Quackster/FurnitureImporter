using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace Helios.Storage
{
    public class StorageContext : DbContext
    {
        #region Properties

        public static MySqlServerVersion ServerVersion => new MySqlServerVersion(new Version(8, 0, 40));

        public DbSet<CatalogueItem> CatalogueItems { get; set; } = null!;
        public DbSet<CataloguePage> CataloguePages { get; set; } = null!;
        public DbSet<ItemDefinition> ItemDefinitions { get; set; } = null!;

        #endregion

        #region Constructor

        public StorageContext()
        {

        }

        public StorageContext(DbContextOptions<StorageContext> options) : base(options)
        {

        }

        #endregion

        #region Public methods

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var _configuration = new ConfigurationBuilder()
                   .AddJsonFile("appsettings.json")
                   .Build();

                var connectionString = _configuration.GetConnectionString("ConnectionString");
                optionsBuilder.UseMySql(connectionString, ServerVersion);
            }

            base.OnConfiguring(optionsBuilder);

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new CatalogueItemConfiguration());
            modelBuilder.ApplyConfiguration(new CataloguePageConfiguration());
            modelBuilder.ApplyConfiguration(new ItemDefinitionConfiguration());
        }
    }

    #endregion
}
