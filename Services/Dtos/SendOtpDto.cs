using System;

namespace Services.Dtos
{
    public class SendOtpDto
    {
        public string OtpCode { get; set; }
        
        public DateTime LifeTime { get; set; }
    }
}