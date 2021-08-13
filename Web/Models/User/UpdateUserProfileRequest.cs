using System;
using Entities.User;

namespace Web.Models
{
    public class UpdateUserProfileRequest
    {
        public string UserName { get; set; }
        
        public string FullName { get; set; }
        
        public string Email { get; set; }
        
        public string PhoneNumber { get; set; }

        public DateTime? BirthDate { get; set; }

        public GenderType? Gender { get; set; }
    }
}