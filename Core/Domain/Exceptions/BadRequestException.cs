namespace Domain.Exceptions;

public sealed class BadRequestException : Exception
{
    public BadRequestException(string message, List<string>? Errors) : base(message)
    {
        Errors = Errors ?? [];
    }
    public BadRequestException(string message) : this( message, null)
    {
        
    }
    public List<string> Errors { get; } = [];
}