namespace api.Exceptions;

public class CreateServerException : ApiException {
    
    public CreateServerException() { }

    public CreateServerException(string message)
        : base(message) { }

    public CreateServerException(string message, Exception inner)
        : base(message, inner) { }
}