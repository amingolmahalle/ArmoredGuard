using System;
using Common.Enums;

namespace Common.Exceptions
{
    public class UnAuthorizedException : AppException
    {
        public UnAuthorizedException()
            : base(ApiResultStatusCodeType.UnAuthorized, System.Net.HttpStatusCode.NotFound)
        {
        }

        public UnAuthorizedException(string message)
            : base(ApiResultStatusCodeType.NotFound, message, System.Net.HttpStatusCode.NotFound)
        {
        }

        public UnAuthorizedException(object additionalData)
            : base(ApiResultStatusCodeType.UnAuthorized, null, System.Net.HttpStatusCode.Unauthorized, additionalData)
        {
        }

        public UnAuthorizedException(string message, object additionalData)
            : base(ApiResultStatusCodeType.UnAuthorized, message, System.Net.HttpStatusCode.Unauthorized, additionalData)
        {
        }

        public UnAuthorizedException(string message, Exception exception)
            : base(ApiResultStatusCodeType.UnAuthorized, message, exception, System.Net.HttpStatusCode.Unauthorized)
        {
        }

        public UnAuthorizedException(string message, Exception exception, object additionalData)
            : base(ApiResultStatusCodeType.UnAuthorized, message, System.Net.HttpStatusCode.Unauthorized, exception, additionalData)
        {
        }
    }
}
