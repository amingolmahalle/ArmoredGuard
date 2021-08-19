using System;
using Common.Enums;

namespace Common.Exceptions
{
    public class LogicException : AppException
    {
        public LogicException()
            : base(ApiResultStatusCodeType.LogicError)
        {
        }

        public LogicException(string message)
            : base(ApiResultStatusCodeType.LogicError, message)
        {
        }

        public LogicException(object additionalData)
            : base(ApiResultStatusCodeType.LogicError, additionalData)
        {
        }

        public LogicException(string message, object additionalData)
            : base(ApiResultStatusCodeType.LogicError, message, additionalData)
        {
        }

        public LogicException(string message, Exception exception)
            : base(ApiResultStatusCodeType.LogicError, message, exception)
        {
        }

        public LogicException(string message, Exception exception, object additionalData)
            : base(ApiResultStatusCodeType.LogicError, message, exception, additionalData)
        {
        }
    }
}