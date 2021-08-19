using System;
using Common.Enums;

namespace Web.Models.ResponseModel.User
{
    public class GetByUserIdResponse
    {
        public string UserName { get; set; }
        
        public string FullName { get; set; }
        
        public string Email { get; set; }
        
        public string PhoneNumber { get; set; }

        public DateTime? BirthDate { get; set; }

        public GenderType Gender { get; set; }
        
        public DateTimeOffset LastLoginDate { get; set; }
    }
}