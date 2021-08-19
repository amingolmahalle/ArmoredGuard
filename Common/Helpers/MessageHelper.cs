using System;

namespace Common.Helpers
{
    public class MessageHelper
    {
        public static string GenerateOtpCode(int min = 1000, int max = 9999)
        {
            Random random = new Random(DateTime.Now.Millisecond);

            return random.Next(min, max).ToString();
        }
    }
}