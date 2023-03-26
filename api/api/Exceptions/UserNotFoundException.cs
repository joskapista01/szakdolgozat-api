namespace api.Exceptions;

public class UserNotFoundException : AuthenticationException {
    
    public UserNotFoundException() { }

    public UserNotFoundException(string message)
        : base(message) { }

    public UserNotFoundException(string message, Exception inner)
        : base(message, inner) { }
}