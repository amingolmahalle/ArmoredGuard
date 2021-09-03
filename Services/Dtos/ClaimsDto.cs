using System.Collections.Generic;

namespace Services.Dtos
{
    public class ClaimsDto
    {
        public int UserId { get; set; }

        public string Username { get; set; }

        public string MobileNumber { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public IList<string> Roles { get; set; }

        public string SecurityStamp { get; set; }
    }
}