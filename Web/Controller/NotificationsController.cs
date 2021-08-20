using System.Threading;
using System.Threading.Tasks;
using Common.Helpers;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts.Redis;
using Web.Controller.Base;

namespace Web.Controller
{
    public class NotificationsController : BaseController
    {
        private readonly IRedisService _redisService;

        public NotificationsController(IRedisService redisService)
        {
            _redisService = redisService;
        }

        [HttpGet("send-otp")]
        public async Task<ApiResult.ApiResult> SendOtp(string phoneNumber, CancellationToken cancellationToken)
        {
            bool hasExistOtpCodeForPhoneNumber = await _redisService.TryGetAsync(phoneNumber, cancellationToken);

            if (hasExistOtpCodeForPhoneNumber)
                return BadRequest("request is duplicate");

            string otpCode = RandomGeneratorHelper.GenerateOtpCode();

            await _redisService.SetAsync(phoneNumber, otpCode, 2, cancellationToken);

            //TODO: send message 

            return Content($"otp Code for {phoneNumber} is: {otpCode}");
        }
    }
}