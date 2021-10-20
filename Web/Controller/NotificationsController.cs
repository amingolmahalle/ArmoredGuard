using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Exceptions;
using Common.Extensions;
using Common.Helpers;
using Entities.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Services.Contracts.Redis;
using Services.Dtos;
using Web.ApiResult;
using Web.Controller.Base;
using Web.Models.RequestModels.Notification;
using Web.Models.ResponseModel.Notification;

namespace Web.Controller
{
    public class NotificationsController : BaseController
    {
        private readonly IRedisService _redisService;

        private readonly IUserService _userService;

        public NotificationsController(IRedisService redisService, IUserService userService)
        {
            _redisService = redisService;
            _userService = userService;
        }

        [HttpPost("create-otp")]
        [AllowAnonymous]
        public async Task<ApiResult<SendOtpResponse>> CreateOtp(SendOtpRequest request, CancellationToken cancellationToken)
        {
            User user = await _userService.GetByPhoneNumberAsync(request.PhoneNumber, cancellationToken);

            if (user is null && !request.IsRegister)
                throw new NotFoundException("user not found");

            SendOtpDto otpResult = await _redisService.GetAsync<SendOtpDto>(request.PhoneNumber, cancellationToken);

            short ttl = 2; // minutes

            if (otpResult is not null)
            {
                DateTime lifeTime = otpResult.LifeTime;
                int otpExpirationTimeInSeconds = (int) (DateTime.Now - lifeTime).TotalSeconds;

                return new SendOtpResponse
                {
                    OtpExpirationTimeSeconds = ttl * 60 - otpExpirationTimeInSeconds
                };
            }

            string otpCode = RandomGeneratorHelper.GenerateOtpCode();

            SendOtpDto newOtpDto = new SendOtpDto {OtpCode = otpCode, LifeTime = DateTime.Now};

            await _redisService.SetAsync(request.PhoneNumber, newOtpDto.Serialize(), ttl, cancellationToken);

            Console.WriteLine($@"OtpCode is: {otpCode}");

            //TODO: send message 

            return new SendOtpResponse {OtpExpirationTimeSeconds = ttl * 60};
        }
    }
}