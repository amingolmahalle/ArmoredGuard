using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Entities.User;

namespace Web.Models
{
    public class UserRequest : IValidatableObject
    {
        [Required] [StringLength(100)] public string UserName { get; set; }

        [Required] [StringLength(100)] public string Email { get; set; }

        [Required] [StringLength(500)] public string Password { get; set; }

        [Required] [StringLength(100)] public string FullName { get; set; }

        public int Age { get; set; }

        public GenderType Gender { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Password.Equals("123456"))
                yield return new ValidationResult("Password cannot be 123456", new[] {nameof(Password)});

            if (Gender == GenderType.Male && Age > 30)
                yield return new ValidationResult("Men more than 30 years are not valid",
                    new[] {nameof(Gender), nameof(Age)});
        }
    }
}