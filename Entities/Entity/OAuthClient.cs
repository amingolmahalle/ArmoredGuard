using System;
using System.Collections.Generic;
using Entities.BaseEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Entity
{
    public class OAuthClient : BaseEntity<int>
    {
        public Guid SecretCode { get; set; }

        public string Name { get; set; }

        public string ResourceServerUri { get; set; }

        public bool Enabled { get; set; }

        public string RedirectUri { get; set; }

        public List<OAuthRefreshToken> OAuthRefreshTokens { get; set; }
    }

    public class OAuthClientConfiguration : IEntityTypeConfiguration<OAuthClient>
    {
        public void Configure(EntityTypeBuilder<OAuthClient> builder)
        {
            builder
                .Property(p => p.SecretCode)
                .IsRequired()
                .HasMaxLength(100);

            builder
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(30);

            builder
                .Property(p => p.ResourceServerUri)
                .IsRequired()
                .HasMaxLength(50);

            builder
                .Property(p => p.RedirectUri)
                .HasMaxLength(200);

            builder
                .HasMany(e => e.OAuthRefreshTokens)
                .WithOne(e => e.OAuthClient)
                .HasForeignKey(f => f.OAuthClientId);
        }
    }
}