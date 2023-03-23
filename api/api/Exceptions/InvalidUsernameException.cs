namespace api.Exceptions;

public class InvalidUsernameException : RegisterUserException {
    
    public InvalidUsernameException() { }

    public InvalidUsernameException(string message)
        : base(message) { }

    public InvalidUsernameException(string message, Exception inner)
        : base(message, inner) { }
}