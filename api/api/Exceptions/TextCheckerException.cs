namespace api.Exceptions;

public class TextCheckerException : ApiException {
    
    public TextCheckerException() { }

    public TextCheckerException(string message)
        : base(message) { }

    public TextCheckerException(string message, Exception inner)
        : base(message, inner) { }
}