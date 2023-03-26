namespace api.Exceptions;

public class ServerNotFoundException : DatabaseException {
    
    public ServerNotFoundException() { }

    public ServerNotFoundException(string message)
        : base(message) { }

    public ServerNotFoundException(string message, Exception inner)
        : base(message, inner) { }
}