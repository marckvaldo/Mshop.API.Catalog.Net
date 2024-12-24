using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mshop.Domain.Entity;

namespace Mshop.Infra.Data.Mapping
{
    public class ProductMapping : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasColumnType("varchar(255)");

            builder.Property(x => x.Description)
                .IsRequired()
                .HasColumnType("text");

            builder.Property(x => x.Price)
                .IsRequired()
                .HasColumnType("decimal(16,2)");

            builder.Property(x => x.Stock)
                .IsRequired()
                .HasColumnType("decimal(16,2)");

            builder.Property(x=>x.IsSale)
                .IsRequired()
                .HasColumnType("bool");

            builder.OwnsOne(x => x.Thumb,
                x => x.Property(x => x.Path).HasColumnName("Thumb"));

            //ignorar a propriedade DomainEvent  que está no aggregateRoot se não ignorar irá aparecer o erro 
            //System.InvalidOperationException : The entity type 'DomainEvent' requires a primary key to be defined. If you intended to use a keyless entity type, call 'HasNoKey' in 'OnModelCreating'. For more information on keyless entity types, see https://go.microsoft.com/fwlink/?linkid=2141943.
            builder.Ignore(x => x.Events);

            builder.ToTable("Products");
        }
    }
}
