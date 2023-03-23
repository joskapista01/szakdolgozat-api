namespace api.Exceptions;

public class InvalidServerNameException : CreateServerException {
    
    public InvalidServerNameException() { }

    public InvalidServerNameException(string message)
        : base(message) { }

    public InvalidServerNameException(string message, Exception inner)
        : base(message, inner) { }
}