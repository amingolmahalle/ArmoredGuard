using Entities.BaseEntity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Entity;

public class Role : IdentityRole<int>, IEntity
{
    public string Description { get; set; }
}

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder
            .Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(30);

        builder
            .Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(50);
    }
}