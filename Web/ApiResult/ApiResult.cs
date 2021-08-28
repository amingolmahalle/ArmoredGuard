using Common.Enums;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Web.ApiResult
{
    public class ApiResult
    {
        public bool IsSuccess { get; set; }
        public ApiResultStatusCodeType StatusCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        public ApiResult(bool isSuccess, ApiResultStatusCodeType statusCode, string message = null)
        {
            IsSuccess = isSuccess;
            StatusCode = statusCode;
            Message = message;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
        }

        #region Implicit Operators

        public static implicit operator ApiResult(OkResult result)
        {
            return new ApiResult(true, ApiResultStatusCodeType.Success);
        }

        public static implicit operator ApiResult(ContentResult result)
        {
            return new(true, ApiResultStatusCodeType.Success, result.Content);
        }

        #endregion
    }

    public class ApiResult<TData> : ApiResult where TData : class
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public TData Data { get; set; }

        public ApiResult(TData data, bool isSuccess, ApiResultStatusCodeType statusCode, string message = null) : base(
            isSuccess, statusCode, message)
        {
            Data = data;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
        }

        #region Implicit Operators

        public static implicit operator ApiResult<TData>(TData data)
        {
            return new(data, true, ApiResultStatusCodeType.Success);
        }

        public static implicit operator ApiResult<TData>(OkResult result)
        {
            return new(null, true, ApiResultStatusCodeType.Success);
        }

        public static implicit operator ApiResult<TData>(OkObjectResult result)
        {
            return new((TData)result.Value, true, ApiResultStatusCodeType.Success);
        }

        public static implicit operator ApiResult<TData>(ContentResult result)
        {
            return new(null, true, ApiResultStatusCodeType.Success, result.Content);
        }

        #endregion
    }
}