
using api.Exceptions;

namespace api.Services.Helpers;
public class UserRegistration
{
    public static void ValidateNewCredentials(string username, string password)
    {
        ValidateUsername(username);
        ValidatePassword(password);
    }
    private static void ValidateUsername(string username)
    {   
        try
        {
            TextChecker.MinLength(username, 4);
            TextChecker.MaxLength(username,16);
            TextChecker.IsAlphaNumeric(username);
        }
        catch(TextCheckerException e)
        {
            throw new InvalidUsernameException("Invalid username! Username " + e.Message);
        }


    }

    private static void ValidatePassword(string password)
    {
        try
        {
            TextChecker.MinLength(password, 8);
            TextChecker.MaxLength(password,24);
            TextChecker.IsAlphaNumeric(password);
            TextChecker.ContainsNumber(password); 
            TextChecker.ContainsSmallLetter(password); 
            TextChecker.ConainsCapitalLetter(password);
        }
        catch(TextCheckerException e)
        {
            throw new InvalidPasswordException("Invalid password! Password " + e.Message);
        }
    }


}