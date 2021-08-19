using System;
using Common.Enums;

namespace Common.Exceptions
{
    public class BadRequestException : AppException
    {
        public BadRequestException()
            : base(ApiResultStatusCodeType.BadRequest, System.Net.HttpStatusCode.BadRequest)
        {
        }

        public BadRequestException(string message)
            : base(ApiResultStatusCodeType.BadRequest, message, System.Net.HttpStatusCode.BadRequest)
        {
        }

        public BadRequestException(object additionalData)
            : base(ApiResultStatusCodeType.BadRequest, null, System.Net.HttpStatusCode.BadRequest, additionalData)
        {
        }

        public BadRequestException(string message, object additionalData)
            : base(ApiResultStatusCodeType.BadRequest, message, System.Net.HttpStatusCode.BadRequest, additionalData)
        {
        }

        public BadRequestException(string message, Exception exception)
            : base(ApiResultStatusCodeType.BadRequest, message, exception, System.Net.HttpStatusCode.BadRequest)
        {
        }

        public BadRequestException(string message, Exception exception, object additionalData)
            : base(ApiResultStatusCodeType.BadRequest, message, System.Net.HttpStatusCode.BadRequest, exception,
                additionalData)
        {
        }
    }
}