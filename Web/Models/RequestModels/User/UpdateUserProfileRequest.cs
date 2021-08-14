using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Common.Helpers;
using Entities.User;

namespace Web.Models.RequestModels.User
{
    public class UpdateUserProfileRequest : IValidatableObject
    {
        public string UserName { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime? BirthDate { get; set; }

        public GenderType? Gender { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!PhoneNumber.IsMobileNumber())
                yield return new ValidationResult("PhoneNumber is invalid", new[] {nameof(PhoneNumber)});

            if (!PhoneNumber.IsValidEmail())
                yield return new ValidationResult("Email Address is invalid", new[] {nameof(Email)});
        }
    }
}