namespace api.Exceptions;

public class UsernameAlreadyTakenException : RegisterUserException {
    
    public UsernameAlreadyTakenException() { }

    public UsernameAlreadyTakenException(string message)
        : base(message) { }

    public UsernameAlreadyTakenException(string message, Exception inner)
        : base(message, inner) { }
}