using Common.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Services.Dtos;
using Web.ApiResult;

namespace Web.Filter
{
    public class ApiResultFilterAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ObjectResult objectResult
                && objectResult.StatusCode == null
                && objectResult.Value is not ApiResult.ApiResult
                && objectResult.Value is not AccessTokenDto)
            {
                var apiResult = new ApiResult<object>(objectResult.Value, true, ApiResultStatusCodeType.Success);
                context.Result = new JsonResult(apiResult) {StatusCode = objectResult.StatusCode};
            }

            base.OnResultExecuting(context);
        }
    }
}