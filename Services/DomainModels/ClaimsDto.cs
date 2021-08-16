using System.Collections.Generic;

namespace Services.DomainModels
{
    public class ClaimsDto
    {
        public int UserId { get; set; }

        public string Username { get; set; }
        
        public string FullName { get; set; }
        
        public IList<string> RolesName { get; set; }
        
        public string SecurityStamp { get; set; }
    }
}