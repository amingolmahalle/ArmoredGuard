using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Web.Models.RequestModels.Identity
{
    public class GetTokenByRefreshCodeRequest
    {
        [JsonProperty("refresh_token")] [Required]
        public Guid RefreshToken { get; set; }

        [JsonProperty("client_id")] [Required] public string ClientId { get; set; }

        [JsonProperty("client_secret")] [Required] public string ClientSecret { get; set; }
    }
}