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

namespace Web.Controller;

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
        User user = await _userService.GetByPhoneNumberAsync(request.PhoneNumber, cancellationToken);

        if (user is null && !request.IsRegister)
            throw new NotFoundException("user not found");

        SendOtpDto otpResult = await _redisService.GetAsync<SendOtpDto>(request.PhoneNumber, cancellationToken);

        if (otpResult is not null && otpResult.ExpiresAt > DateTime.Now)
        {
            return new SendOtpResponse
            {
                OtpExpirationTimeSeconds = (int) (otpResult.ExpiresAt - DateTime.Now).TotalSeconds,
            };
        }

        const short ttlSeconds = 2 * 60;
        string otpCode = Randomizer.GenerateOtpCode(10000, 99999);

        SendOtpDto newOtpDto = new SendOtpDto
        {
            OtpCode = otpCode,
            ExpiresAt = DateTime.Now.AddSeconds(ttlSeconds)
        };

        await _redisService.SetAsync(request.PhoneNumber, newOtpDto.Serialize(), ttlSeconds, cancellationToken);

        Console.WriteLine($@"OtpCode is: {otpCode}");

        //TODO: send message 

        return new SendOtpResponse {OtpExpirationTimeSeconds = ttlSeconds};
    }
}