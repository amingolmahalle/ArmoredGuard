namespace Web.Models
{
    public class ChangeUserPasswordRequest
    {
        public string CurrentPassword { get; set; }
        
        public string NewPassword { get; set; }
    }
}