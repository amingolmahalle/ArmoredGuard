using System;
using Entities.BaseEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.OAuth
{
    public class OAuthRefreshToken : BaseEntity<int>
    {
        public int CreatedBy { get; set; }
        public User.User User { get; set; }
        public int OAuthClientId { get; set; }
        public OAuthClient OAuthClient { get; set; }
        public Guid RefreshCode { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset ExpiresAt { get; set; }
    }

    public class OAuthRefreshTokenConfiguration : IEntityTypeConfiguration<OAuthRefreshToken>
    {
        public void Configure(EntityTypeBuilder<OAuthRefreshToken> builder)
        {
            builder.Property(p => p.RefreshCode).IsRequired();
        }
    }
}