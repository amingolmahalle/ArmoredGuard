using Entities.User;

namespace Web.Models
{
    public class UpdateUserRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public GenderType Gender { get; set; }
        public bool IsActive { get; set; }
    }
}