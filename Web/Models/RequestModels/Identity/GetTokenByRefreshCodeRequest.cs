using System;
using System.ComponentModel.DataAnnotations;

namespace Web.Models.RequestModels.Identity
{
    public class GetTokenByRefreshCodeRequest
    {
        [Required] public Guid refresh_token { get; set; }

        [Required] public string client_id { get; set; }

        [Required] public string client_secret { get; set; }
    }
}