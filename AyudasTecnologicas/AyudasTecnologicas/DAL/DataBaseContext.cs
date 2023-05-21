using AyudasTecnologicas.DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AyudasTecnologicas.DAL
{
    public class DataBaseContext : IdentityDbContext<User>
    {
        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
        {
        }

        //Aquí estoy mapeando mi entidad para convertirla en un DBSet (tabla)
        public DbSet<Country> Countries { get; set; } //La tabla se debe llamar en plural: Countries
        public DbSet<Services> Categories { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<TechnicalServices> Products { get; set; }
        public DbSet<ServicesCategory> ProductCategories { get; set; }
        public DbSet<ServicesImage> ProductImages { get; set; }
        public DbSet<TemporalService> TemporalSales { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetailservices> OrderDetails { get; set; }

        //Vamos a crear un índice para la tabla Countries
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Country>().HasIndex(c => c.Name).IsUnique();
            modelBuilder.Entity<Services>().HasIndex(c => c.Name).IsUnique();
            modelBuilder.Entity<State>().HasIndex("Name", "CountryId").IsUnique(); // Para estos casos, debo crear un índice Compuesto
            modelBuilder.Entity<City>().HasIndex("Name", "StateId").IsUnique(); // Para estos casos, debo crear un índice Compuesto
            modelBuilder.Entity<TechnicalServices>().HasIndex(c => c.Name).IsUnique();
            modelBuilder.Entity<ServicesCategory>().HasIndex("ProductId", "CategoryId").IsUnique();
        }
    }
}
