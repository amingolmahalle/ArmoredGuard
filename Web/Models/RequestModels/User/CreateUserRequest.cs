using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Common.Extensions;
using Common.Helpers.Enums;
using Entities.Entity;

namespace Web.Models.RequestModels.User
{
    public class CreateUserRequest : IValidatableObject
    {
        [Required] [StringLength(100)] public string UserName { get; set; }

        [StringLength(100)] public string Email { get; set; }

        [Required] [StringLength(11)] public string PhoneNumber { get; set; }

        [StringLength(200)] public string Password { get; set; }

        [StringLength(4)] public string OtpCode { get; set; }

        [StringLength(100)] public string FullName { get; set; }

        [Required] public RegisterType RegisterType { get; set; }

        public int RoleId { get; set; }

        public DateTime? BirthDate { get; set; }

        public GenderType Gender { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!PhoneNumber.IsPhoneNumber())
                yield return new ValidationResult("PhoneNumber is invalid", new[] {nameof(PhoneNumber)});

            if (!string.IsNullOrEmpty(Email) && !Email.IsValidEmail())
                yield return new ValidationResult("Email Address is invalid", new[] {nameof(Email)});

            if (UserName.Length < 4)
                yield return new ValidationResult("Username must be longer than 4 characters",
                    new[] {nameof(UserName)});
        }
    }
}