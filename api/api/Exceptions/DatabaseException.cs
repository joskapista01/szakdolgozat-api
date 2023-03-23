namespace api.Exceptions;

public class DatabaseException : ApiException {
    
    public DatabaseException() { }

    public DatabaseException(string message)
        : base(message) { }

    public DatabaseException(string message, Exception inner)
        : base(message, inner) { }
}