using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Common.Extensions;

namespace Web.Models.RequestModels.Notification
{
    public class SendOtpRequest : IValidatableObject
    {
        public string phoneNumber { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (phoneNumber is null)
                yield return new ValidationResult("GrantType is invalid", new[] {nameof(phoneNumber)});

            if (phoneNumber is not null && !phoneNumber.IsPhoneNumber())
            {
                yield return new ValidationResult("phoneNumber is incorrect", new[] {nameof(phoneNumber)});
            }
        }
    }
}