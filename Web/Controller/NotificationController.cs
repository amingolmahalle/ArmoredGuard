using System.Threading;
using System.Threading.Tasks;
using Common.Helpers;
using Microsoft.AspNetCore.Mvc;
using Web.Controller.Base;
using WebFramework.ApiResult;
using WebFramework.Caching.Redis;

namespace Web.Controller
{
    public class NotificationController : BaseController
    {
        private readonly IRedisService _redisService;

        public NotificationController(IRedisService redisService)
        {
            _redisService = redisService;
        }

        [HttpGet("send-otp")]
        public async Task<ApiResult> SendOtp(string phoneNumber, CancellationToken cancellationToken)
        {
            string otpCode = MessageHelper.GenerateOtpCode();

            await _redisService.SetAsync(phoneNumber, otpCode, 2, cancellationToken);

            //TODO: send message 

            return Content($"otp Code for {phoneNumber} is: {otpCode}");
        }
    }
}