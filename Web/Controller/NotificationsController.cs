using System.Threading;
using System.Threading.Tasks;
using Common.Exceptions;
using Common.Helpers;
using Microsoft.AspNetCore.Authorization;
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

        [HttpPost("create-otp")]
        [AllowAnonymous]
        public async Task<ApiResult.ApiResult> CreateOtp(string phoneNumber, CancellationToken cancellationToken)
        {
            bool hasExistOtpCodeForPhoneNumber = await _redisService.TryGetAsync(phoneNumber, cancellationToken);

            if (hasExistOtpCodeForPhoneNumber)
                throw new BadRequestException("request is duplicate");

            string otpCode = RandomGeneratorHelper.GenerateOtpCode();

            await _redisService.SetAsync(phoneNumber, otpCode, 2, cancellationToken);

            //TODO: send message 

            return Content(otpCode);
        }
    }
}