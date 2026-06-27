namespace Hasin.Model.Exceptions;

public class DomainValidationException : Exception
{
    public string message { get; }

    public DomainValidationException(string message)
    {
        this.message = message;
    }
}