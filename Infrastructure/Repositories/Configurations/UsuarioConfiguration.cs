using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Repositories.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {

        public void Configure(EntityTypeBuilder<Usuario> builder)
        {

            builder.ToTable("Usuario");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnType("INT").UseIdentityColumn();
            builder.Property(c => c.DataInclusao).HasColumnName("DataInclusao").HasColumnType("DATETIME").IsRequired();
            builder.Property(c => c.Nome).HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(r => r.Email).HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(c => c.Senha).HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(c => c.Role).HasColumnType("VARCHAR(50)").IsRequired();

   
        }

    }
}
