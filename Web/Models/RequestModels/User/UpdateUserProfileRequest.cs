using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Common.Extensions;
using Entities.Entity;

namespace Web.Models.RequestModels.User
{
    public class UpdateUserProfileRequest : IValidatableObject
    {
        public string FullName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime? BirthDate { get; set; }

        public GenderType? Gender { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(PhoneNumber) && !PhoneNumber.IsPhoneNumber())
                yield return new ValidationResult("phone number is invalid", new[] {nameof(PhoneNumber)});

            if (!string.IsNullOrEmpty(Email) && !PhoneNumber.IsValidEmail())
                yield return new ValidationResult("email address is invalid", new[] {nameof(Email)});
        }
    }
}