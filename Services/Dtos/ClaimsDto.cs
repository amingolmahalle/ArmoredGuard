using System.Collections.Generic;

namespace Services.Dtos
{
    public class ClaimsDto
    {
        public int UserId { get; set; }

        public string Username { get; set; }
        
        public IList<string> Roles { get; set; }
        
        public string SecurityStamp { get; set; }
    }
}