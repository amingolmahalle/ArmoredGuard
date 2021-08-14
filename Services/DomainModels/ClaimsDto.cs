namespace Services.DomainModels
{
    public class ClaimsDto
    {
        public int UserId { get; set; }

        public string Username { get; set; }
        
        public string FullName { get; set; }
        
        public string RoleName { get; set; }
        
        public string SecurityStampClaim { get; set; }
    }
}