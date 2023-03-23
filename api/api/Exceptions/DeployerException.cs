namespace api.Exceptions;

public class DeployerException : ApiException {
    
    public DeployerException() { }

    public DeployerException(string message)
        : base(message) { }

    public DeployerException(string message, Exception inner)
        : base(message, inner) { }
}