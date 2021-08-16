using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Common.Extensions;
using Common.Helpers;
using Entities.User;

namespace Web.Models.RequestModels.User
{
    public class CreateUserRequest : IValidatableObject
    {
        [Required] [StringLength(100)] public string UserName { get; set; }

        [Required] [StringLength(100)] public string Email { get; set; }

        [StringLength(11)] public string PhoneNumber { get; set; }

        [Required] [StringLength(500)] public string Password { get; set; }

        [Required] [StringLength(100)] public string FullName { get; set; }

        public int RoleId { get; set; }

        public DateTime? BirthDate { get; set; }

        public GenderType Gender { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!PhoneNumber.IsMobileNumber())
                yield return new ValidationResult("PhoneNumber is invalid", new[] {nameof(PhoneNumber)});
            
            if (!Email.IsValidEmail())
                yield return new ValidationResult("Email Address is invalid", new[] {nameof(Email)});
        }
    }
}