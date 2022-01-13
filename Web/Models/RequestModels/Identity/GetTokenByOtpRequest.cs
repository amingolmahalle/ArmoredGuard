using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Common.Extensions;

namespace Web.Models.RequestModels.Identity;

public class GetTokenByOtpRequest : IValidatableObject
{
    [Required] public string grant_type { get; set; }
        
    [Required] public string phone_number { get; set; }

    [Required] public string otp_code { get; set; }

    [Required] public string client_id { get; set; }

    [Required] public string client_secret { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!grant_type.Equals("otp", StringComparison.OrdinalIgnoreCase))
            yield return new ValidationResult("GrantType is invalid", new[] {nameof(grant_type)});
            
        if (!phone_number.IsPhoneNumber())
            yield return new ValidationResult("PhoneNumber is invalid", new[] {nameof(phone_number)});
    }
}