using System.Linq;
using Common.Helpers.Enums;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Web.ApiResult
{
    public class ApiResult
    {
        public bool IsSuccess { get; set; }
        public ApiResultStatusCode StatusCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        public ApiResult(bool isSuccess, ApiResultStatusCode statusCode, string message = null)
        {
            IsSuccess = isSuccess;
            StatusCode = statusCode;
            Message = message;
        }

        #region Implicit Operators

        public static implicit operator ApiResult(OkResult result)
        {
            return new ApiResult(true, ApiResultStatusCode.Success);
        }

        public static implicit operator ApiResult(BadRequestResult result)
        {
            return new ApiResult(true, ApiResultStatusCode.BadRequest);
        }

        public static implicit operator ApiResult(BadRequestObjectResult result)
        {
            var message = result.Value?.ToString();
            if (result.Value is SerializableError errors)
            {
                var errorMessages = errors.SelectMany(p => (string[]) p.Value).Distinct();
                message = string.Join(" | ", errorMessages);
            }

            return new ApiResult(false, ApiResultStatusCode.BadRequest, message);
        }

        public static implicit operator ApiResult(ContentResult result)
        {
            return new(true, ApiResultStatusCode.Success, result.Content);
        }

        public static implicit operator ApiResult(NotFoundResult result)
        {
            return new(false, ApiResultStatusCode.NotFound);
        }

        public static implicit operator ApiResult(NoContentResult result)
        {
            return new(true, ApiResultStatusCode.NoContent);
        }

        public static implicit operator ApiResult(NotFoundObjectResult result)
        {
            return new(false, ApiResultStatusCode.NotFound, result.Value?.ToString());
        }

        #endregion
    }

    public class ApiResult<TData> : ApiResult where TData : class 
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public TData Data { get; set; }

        public ApiResult(TData data, bool isSuccess, ApiResultStatusCode statusCode, string message = null) : base(
            isSuccess, statusCode, message)
        {
            Data = data;
        }

        #region Implicit Operators

        public static implicit operator ApiResult<TData>(TData data)
        {
            return new(data, true, ApiResultStatusCode.Success);
        }

        public static implicit operator ApiResult<TData>(OkResult result)
        {
            return new(null, true, ApiResultStatusCode.Success);
        }

        public static implicit operator ApiResult<TData>(OkObjectResult result)
        {
            return new((TData) result.Value, true, ApiResultStatusCode.Success);
        }

        public static implicit operator ApiResult<TData>(BadRequestResult result)
        {
            return new(null, false, ApiResultStatusCode.BadRequest);
        }

        public static implicit operator ApiResult<TData>(BadRequestObjectResult result)
        {
            var message = result.Value?.ToString();

            if (result.Value is SerializableError errors)
            {
                var errorMessages = errors.SelectMany(p => (string[]) p.Value)
                    .Distinct();

                message = string.Join(" | ", errorMessages);
            }

            return new ApiResult<TData>(null, false, ApiResultStatusCode.BadRequest, message);
        }

        public static implicit operator ApiResult<TData>(ContentResult result)
        {
            return new(null, true, ApiResultStatusCode.Success, result.Content);
        }

        public static implicit operator ApiResult<TData>(NoContentResult result)
        {
            return new(null, true, ApiResultStatusCode.NoContent);
        }

        public static implicit operator ApiResult<TData>(NotFoundResult result)
        {
            return new(null, false, ApiResultStatusCode.NotFound);
        }

        public static implicit operator ApiResult<TData>(NotFoundObjectResult result)
        {
            return new((TData) result.Value, false, ApiResultStatusCode.NotFound);
        }

        #endregion
    }
}