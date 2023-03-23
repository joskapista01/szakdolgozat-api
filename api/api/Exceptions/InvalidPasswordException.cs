namespace api.Exceptions;

public class InvalidPasswordException : RegisterUserException {
    
    public InvalidPasswordException() { }

    public InvalidPasswordException(string message)
        : base(message) { }

    public InvalidPasswordException(string message, Exception inner)
        : base(message, inner) { }
}