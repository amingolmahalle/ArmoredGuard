using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Common.Extensions;
using Newtonsoft.Json;

namespace Web.Models.RequestModels.Identity
{
    public class GetTokenByOtpRequest : IValidatableObject
    {
        [JsonProperty("grant_type")] [Required] public string GrantType { get; set; }
        
        [JsonProperty("phone_number")] [Required] public string PhoneNumber { get; set; }

        [JsonProperty("otp_code")] [Required] public string OtpCode { get; set; }

        [JsonProperty("client_id")] [Required] public string ClientId { get; set; }

        [JsonProperty("client_secret")] [Required] public string ClientSecret { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!GrantType.Equals("otp", StringComparison.OrdinalIgnoreCase))
                yield return new ValidationResult("GrantType is invalid", new[] {nameof(GrantType)});
            
            if (!PhoneNumber.IsPhoneNumber())
                yield return new ValidationResult("PhoneNumber is invalid", new[] {nameof(PhoneNumber)});
        }
    }
}