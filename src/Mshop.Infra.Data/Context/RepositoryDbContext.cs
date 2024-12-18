using Microsoft.EntityFrameworkCore;
using Mshop.Domain.Entity;

namespace Mshop.Infra.Data.Context
{
    public class RepositoryDbContext : DbContext
    {
        public RepositoryDbContext(DbContextOptions<RepositoryDbContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Image> Images { get; set; }


        //quando inicar a criação ele vai pegar todas as classes que herdam IEntityTypeConfiguration
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            foreach(var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e=>e.GetProperties()
                .Where(p=>p.ClrType == typeof(string))))
            {
                property.SetColumnType("Varchar(100)");
            }
            
            //modelBuilder.Ignore<DomainEvent>();

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(RepositoryDbContext).Assembly);

            //aqui eu estou retirando o modo cascata
            foreach(var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e=>e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.ClientNoAction;
            }

            base.OnModelCreating(modelBuilder);
        }

        
    }
}
