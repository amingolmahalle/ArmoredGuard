using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Entities.BaseEntity;
using Entities.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.User
{
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
                .HasMaxLength(100);

            builder
                .Property(p => p.UserName)
                .IsRequired()
                .HasMaxLength(100);

            builder
                .HasMany(e => e.OAuthRefreshTokens)
                .WithOne(e => e.User)
                .HasForeignKey(f => f.CreatedBy);
            
            
            //TODO: Constraint on Mobile Number(unique)
        }
    }

    public enum GenderType
    {
        [Display(Name = "Male")] Male = 1,

        [Display(Name = "Female")] Female = 2
    }
}