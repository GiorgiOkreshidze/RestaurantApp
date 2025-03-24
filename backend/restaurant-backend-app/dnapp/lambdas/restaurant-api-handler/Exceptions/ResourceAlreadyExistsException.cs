using System;

namespace Function.Exceptions;

class ResourceAlreadyExistsException : Exception
{
    public ResourceAlreadyExistsException()
    {
    }

    public ResourceAlreadyExistsException(string? message) : base(message)
    {
    }

    public ResourceAlreadyExistsException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}