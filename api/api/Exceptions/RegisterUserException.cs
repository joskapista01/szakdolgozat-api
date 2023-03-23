namespace api.Exceptions;

public class RegisterUserException : Exception {
    
    public RegisterUserException() { }

    public RegisterUserException(string message)
        : base(message) { }

    public RegisterUser(string message, Exception inner)
        : base(message, inner) { }
}