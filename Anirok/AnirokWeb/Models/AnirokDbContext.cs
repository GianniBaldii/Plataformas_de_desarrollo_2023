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

        internal static string ResolveConnectionString()
        {
            // Desarrollo local: la Web siempre usa el SQL Server publicado por
            // Docker. Así IIS Express no puede recuperar una cadena vieja de
            // LocalDB o una contraseña heredada del entorno.
            return "Server=localhost,1433;Database=anirok2;User Id=sa;Password=Anirok_dev_2026!;TrustServerCertificate=True;";
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }


        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Producto> Productos { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetallesVenta { get; set; }

    }
}
