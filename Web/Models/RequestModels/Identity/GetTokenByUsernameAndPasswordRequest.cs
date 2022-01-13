using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.Models.RequestModels.Identity;

public class GetTokenByUsernameAndPasswordRequest : IValidatableObject
{
    [Required] public string grant_type { get; set; }

    [Required] public string username { get; set; }

    [Required] public string password { get; set; }

    [Required] public string client_id { get; set; }

    [Required] public string client_secret { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!grant_type.Equals("password", StringComparison.OrdinalIgnoreCase))
            yield return new ValidationResult("GrantType is invalid", new[] { nameof(grant_type) });
    }
}