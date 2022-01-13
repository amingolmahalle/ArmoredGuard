using System;
using System.Collections.Generic;
using Common.Enums;
using Entities.BaseEntity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Entity;

public class User : IdentityUser<int>, IEntity
{
    public User()
    {
        IsActive = true;
    }

    public string FullName { get; set; }

    public DateTime? BirthDate { get; set; }

    public GenderType Gender { get; set; }

    public bool IsActive { get; set; }

    public DateTimeOffset? LastSeenDate { get; set; }

    public List<OAuthRefreshToken> OAuthRefreshTokens { get; set; }
}

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .Property(p => p.FullName)
            .IsRequired()
            .HasMaxLength(50);

        builder
            .Property(p => p.UserName)
            .IsRequired()
            .HasMaxLength(50);

        builder
            .Property(p => p.PhoneNumber)
            .IsRequired()
            .HasMaxLength(11);

        builder
            .HasIndex(p => p.PhoneNumber)
            .IsUnique();

        builder
            .HasIndex(p => p.Email)
            .IsUnique();

        builder
            .HasMany(e => e.OAuthRefreshTokens)
            .WithOne(e => e.User)
            .HasForeignKey(f => f.CreatedBy);
    }
}