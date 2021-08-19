using Common.Helpers.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Web.ApiResult;

namespace Web.Filter
{
    public class ApiResultFilterAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ObjectResult objectResult
                && objectResult.StatusCode == null
                && objectResult.Value is not ApiResult.ApiResult)
            {
                var apiResult = new ApiResult<object>(objectResult.Value, true, ApiResultStatusCode.Success);
                context.Result = new JsonResult(apiResult) {StatusCode = objectResult.StatusCode};
            }

            base.OnResultExecuting(context);
        }
    }
}