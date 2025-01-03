using Application.DTOs;

namespace Application.Exceptions;

public class DuplicateException : ApplicationException
{
    public DuplicateException(string message): base(ApiResultErrorCodes.Conflict, message)
    {
    }
}