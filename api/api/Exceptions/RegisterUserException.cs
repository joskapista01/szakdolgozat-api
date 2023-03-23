namespace api.Exceptions;

public class RegisterUserException : ApiException {
    
    public RegisterUserException() { }

    public RegisterUserException(string message)
        : base(message) { }

    public RegisterUserException(string message, Exception inner)
        : base(message, inner) { }
}