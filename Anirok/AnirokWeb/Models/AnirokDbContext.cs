using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Configuration;

namespace AnirokWeb.Models
{
    public class AnirokDbContext : DbContext
    {
        public AnirokDbContext() : base(ResolveConnectionString())
        {
            Database.SetInitializer<AnirokDbContext>(null);
            this.Configuration.LazyLoadingEnabled = false;
        }

        private static string ResolveConnectionString()
        {
            var environmentConnection = Environment.GetEnvironmentVariable("ANIROK_DB_CONNECTION");
            if (!string.IsNullOrWhiteSpace(environmentConnection))
            {
                return environmentConnection;
            }

            var configuredConnection = ConfigurationManager.ConnectionStrings["AnirokDbContext"];
            if (configuredConnection == null || string.IsNullOrWhiteSpace(configuredConnection.ConnectionString))
            {
                throw new ConfigurationErrorsException("Definí ANIROK_DB_CONNECTION antes de iniciar AnirokWeb.");
            }

            return configuredConnection.ConnectionString;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }


        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Producto> Productos { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }

    }
}
