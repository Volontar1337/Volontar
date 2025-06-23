namespace Application.Exceptions;

public class DuplicateEmailException : Exception
{
    public DuplicateEmailException(string email)
        : base($"The email '{email}' is already registered.")
    {
    }
}
