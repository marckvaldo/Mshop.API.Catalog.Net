using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mshop.Domain.Entity;

namespace Mshop.Infra.Data.Mapping
{
    public class ImageMapping : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.FileName)
                .IsRequired()
                .HasColumnType("varchar(255)");

            //ignorar a propriedade DomainEvent  que está no aggregateRoot se não ignorar irá aparecer o erro 
            //System.InvalidOperationException : The entity type 'DomainEvent' requires a primary key to be defined. If you intended to use a keyless entity type, call 'HasNoKey' in 'OnModelCreating'. For more information on keyless entity types, see https://go.microsoft.com/fwlink/?linkid=2141943.
            builder.Ignore(x => x.Events);

            builder.ToTable("Images");
        }
    }
}
