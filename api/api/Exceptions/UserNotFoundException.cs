namespace api.Exceptions;

public class UserNotFoundException : DatabaseException {
    
    public UserNotFoundException() { }

    public UserNotFoundException(string message)
        : base(message) { }

    public UserNotFoundException(string message, Exception inner)
        : base(message, inner) { }
}