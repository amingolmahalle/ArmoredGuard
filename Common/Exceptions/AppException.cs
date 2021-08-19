using System;
using System.Net;
using Common.Enums;

namespace Common.Exceptions
{
    public class AppException : Exception
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        
        public ApiResultStatusCodeType ApiStatusCode { get; set; }
        
        public object AdditionalData { get; set; }

        public AppException()
            : this(ApiResultStatusCodeType.ServerError)
        {
        }

        public AppException(ApiResultStatusCodeType statusCode)
            : this(statusCode, null)
        {
        }

        public AppException(string message)
            : this(ApiResultStatusCodeType.ServerError, message)
        {
        }

        public AppException(ApiResultStatusCodeType statusCode, string message)
            : this(statusCode, message, HttpStatusCode.InternalServerError)
        {
        }

        public AppException(string message, object additionalData)
            : this(ApiResultStatusCodeType.ServerError, message, additionalData)
        {
        }

        public AppException(ApiResultStatusCodeType statusCode, object additionalData)
            : this(statusCode, null, additionalData)
        {
        }

        public AppException(ApiResultStatusCodeType statusCode, string message, object additionalData)
            : this(statusCode, message, HttpStatusCode.InternalServerError, additionalData)
        {
        }

        public AppException(ApiResultStatusCodeType statusCode, string message, HttpStatusCode httpStatusCode)
            : this(statusCode, message, httpStatusCode, null)
        {
        }

        public AppException(ApiResultStatusCodeType statusCode, string message, HttpStatusCode httpStatusCode,
            object additionalData)
            : this(statusCode, message, httpStatusCode, null, additionalData)
        {
        }

        public AppException(string message, Exception exception)
            : this(ApiResultStatusCodeType.ServerError, message, exception)
        {
        }

        public AppException(string message, Exception exception, object additionalData)
            : this(ApiResultStatusCodeType.ServerError, message, exception, additionalData)
        {
        }

        public AppException(ApiResultStatusCodeType statusCode, string message, Exception exception)
            : this(statusCode, message, HttpStatusCode.InternalServerError, exception)
        {
        }

        public AppException(ApiResultStatusCodeType statusCode, string message, Exception exception,
            object additionalData)
            : this(statusCode, message, HttpStatusCode.InternalServerError, exception, additionalData)
        {
        }

        public AppException(ApiResultStatusCodeType statusCode, string message, HttpStatusCode httpStatusCode,
            Exception exception)
            : this(statusCode, message, httpStatusCode, exception, null)
        {
        }

        public AppException(ApiResultStatusCodeType statusCode, string message, HttpStatusCode httpStatusCode,
            Exception exception, object additionalData)
            : base(message, exception)
        {
            ApiStatusCode = statusCode;
            HttpStatusCode = httpStatusCode;
            AdditionalData = additionalData;
        }
    }
}