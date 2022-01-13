using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Common.Extensions;

namespace Web.Models.RequestModels.Notification;

public class SendOtpRequest : IValidatableObject
{
    public string PhoneNumber { get; set; }

    public bool IsRegister { get; set; } = false;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (PhoneNumber is null)
            yield return new ValidationResult("GrantType is invalid", new[] {nameof(PhoneNumber)});

        if (PhoneNumber is not null && !PhoneNumber.IsPhoneNumber())
        {
            yield return new ValidationResult("phoneNumber is incorrect", new[] {nameof(PhoneNumber)});
        }
    }
}