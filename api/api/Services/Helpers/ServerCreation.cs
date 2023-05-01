using api.Exceptions;

namespace api.Services.Helpers;

public class ServerCreation
{
    public static void ValidateServerName(string serverName)
    {
        try
        {
            TextChecker.MinLength(serverName, 1); 
            TextChecker.MaxLength(serverName, 24);
            TextChecker.IsAlphaNumeric(serverName);
        }
        catch(TextCheckerException e)
        {
            throw new InvalidServerNameException("Invalid server name! Server name " + e.Message);
        }
    }
}