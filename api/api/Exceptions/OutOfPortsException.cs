namespace api.Exceptions;

public class OutOfPortsException : ApiException {
    
    public OutOfPortsException() { }

    public OutOfPortsException(string message)
        : base(message) { }

    public OutOfPortsException(string message, Exception inner)
        : base(message, inner) { }
}