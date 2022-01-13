using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.Models.RequestModels.User;

public class ChangeUserPasswordRequest : IValidatableObject
{
    public string CurrentPassword { get; set; }

    public string NewPassword { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CurrentPassword == NewPassword)
        {
            yield return new ValidationResult("PhoneNumber is invalid",
                new[] {nameof(CurrentPassword), nameof(NewPassword)});
        }
    }
}