namespace api.Exceptions;

public class MonitorException : ApiException {
    
    public MonitorException() { }

    public MonitorException(string message)
        : base(message) { }

    public MonitorException(string message, Exception inner)
        : base(message, inner) { }
}