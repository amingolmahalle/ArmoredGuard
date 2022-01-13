using System;

namespace Services.Dtos
{
    public class SendOtpDto
    {
        public string OtpCode { get; set; }
        
        public DateTime ExpiresAt { get; set; }
    }
}