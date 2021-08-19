using System;
using System.Net;
using System.Threading.Tasks;
using Common.Enums;
using Microsoft.AspNetCore.Http;

namespace Web.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await _handleExceptionAsync(httpContext, ex);
            }
        }

        private Task _handleExceptionAsync(HttpContext httpContext, Exception ex)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int) HttpStatusCode.InternalServerError;

            string message = "Internal Server Error";
            string response = new ApiResult.ApiResult
            (
                false, ApiResultStatusCodeType.ServerError, message
            ).ToString();

            return httpContext.Response.WriteAsync(response);
        }
    }
}