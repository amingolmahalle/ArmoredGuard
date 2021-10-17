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

        [HttpGet("send-otp")]
        [AllowAnonymous]
        public async Task<ApiResult<SendOtpResponse>> SendOtp(SendOtpRequest request, CancellationToken cancellationToken)
        {
            User user = await _userService.GetByPhoneNumberAsync(request.phoneNumber, cancellationToken);

            if (user is null)
                throw new NotFoundException("user not found");

            SendOtpDto getOtpDto = await _redisService.GetAsync<SendOtpDto>(request.phoneNumber, cancellationToken);

            if (getOtpDto is not null)
                return new SendOtpResponse
                {
                    OtpExpireTimeSeconds = (DateTime.Now - getOtpDto.LifeTime).Seconds
                };

            string otpCode = RandomGeneratorHelper.GenerateOtpCode();
            short ttl = 2; // minutes
            SendOtpDto newOtpDto = new SendOtpDto {OtpCode = otpCode, LifeTime = DateTime.Now};

            await _redisService.SetAsync(request.phoneNumber, newOtpDto.Serialize(), ttl, cancellationToken);

            Console.WriteLine($@"OtpCode is: {otpCode}");
            //TODO: send message 

            return new SendOtpResponse {OtpExpireTimeSeconds = ttl * 60};
        }
    }
}