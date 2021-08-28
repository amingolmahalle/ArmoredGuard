using System;
using System.Net;
using System.Threading.Tasks;
using Common.Enums;
using Common.Exceptions;
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
            catch (BadRequestException ex)
            {
                await _badRequestExceptionAsync(httpContext, ex);
            }
            catch (NotFoundException ex)
            {
                await _notFoundExceptionAsync(httpContext, ex);
            }
            catch (UnAuthorizedException ex)
            {
                await _unAuthorizedExceptionAsync(httpContext, ex);
            }
            catch (Exception ex)
            {
                await _unHandleExceptionAsync(httpContext, ex);
            }
        }

        private Task _unHandleExceptionAsync(HttpContext httpContext, Exception ex)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            string message = "Internal Server Error";
            string response = new ApiResult.ApiResult
            (
                false, ApiResultStatusCodeType.ServerError, message
            ).ToString();

            return httpContext.Response.WriteAsync(response);
        }

        private Task _badRequestExceptionAsync(HttpContext httpContext, Exception ex)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            string message = $"{ex.Message} - {ex.InnerException}";
            string response = new ApiResult.ApiResult
            (
                false, ApiResultStatusCodeType.BadRequest, message
            ).ToString();

            return httpContext.Response.WriteAsync(response);
        }

        private Task _notFoundExceptionAsync(HttpContext httpContext, Exception ex)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;

            string message = $"{ex.Message} - {ex.InnerException}";
            string response = new ApiResult.ApiResult
            (
                false, ApiResultStatusCodeType.NotFound, message
            ).ToString();

            return httpContext.Response.WriteAsync(response);
        }

        private Task _unAuthorizedExceptionAsync(HttpContext httpContext, Exception ex)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

            string message = $"{ex.Message} - {ex.InnerException}";
            string response = new ApiResult.ApiResult
            (
                false, ApiResultStatusCodeType.UnAuthorized, message
            ).ToString();

            return httpContext.Response.WriteAsync(response);
        }
    }
}