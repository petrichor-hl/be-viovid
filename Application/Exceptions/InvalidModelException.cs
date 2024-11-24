using Application.DTOs;

namespace Application.Exceptions;

public class InvalidModelException : ApplicationException
{
    public InvalidModelException(string message) : base(ApiResultErrorCodes.ModelValidation, message)
    {
    }
}

