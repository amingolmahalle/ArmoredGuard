using System;
using Common.Enums;

namespace Common.Exceptions;

public class NotFoundException : AppException
{
    public NotFoundException()
        : base(ApiResultStatusCodeType.NotFound, System.Net.HttpStatusCode.NotFound)
    {
    }

    public NotFoundException(string message)
        : base(ApiResultStatusCodeType.NotFound, message, System.Net.HttpStatusCode.NotFound)
    {
    }

    public NotFoundException(object additionalData)
        : base(ApiResultStatusCodeType.NotFound, null, System.Net.HttpStatusCode.NotFound, additionalData)
    {
    }

    public NotFoundException(string message, object additionalData)
        : base(ApiResultStatusCodeType.NotFound, message, System.Net.HttpStatusCode.NotFound, additionalData)
    {
    }

    public NotFoundException(string message, Exception exception)
        : base(ApiResultStatusCodeType.NotFound, message, exception, System.Net.HttpStatusCode.NotFound)
    {
    }

    public NotFoundException(string message, Exception exception, object additionalData)
        : base(ApiResultStatusCodeType.NotFound, message, System.Net.HttpStatusCode.NotFound, exception, additionalData)
    {
    }
}