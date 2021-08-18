using System;
using Entities.BaseEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.OAuth
{
    public class OAuthClient : BaseEntity<int>
    {
        public Guid SecretCode { get; set; }

        public string Name { get; set; }

        public string ResourceServerUri { get; set; }

        public bool Enabled { get; set; }

        public string RedirectUri { get; set; }

        public OAuthRefreshToken OAuthRefreshToken { get; set; }
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
                .HasMaxLength(300);

            builder
                .HasOne(e => e.OAuthRefreshToken)
                .WithOne(e => e.OAuthClient)
                .HasForeignKey<OAuthRefreshToken>(f => f.OAuthClientId);
        }
    }
}