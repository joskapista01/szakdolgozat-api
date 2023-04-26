using api.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Data.Common;
using Microsoft.AspNetCore.Authentication;

namespace api.Handlers;

/// <summary>
/// Class that handles exceptions related to authentication.
/// </summary>
public class AuthenticationExceptionHandler
{
    /// <summary>
    /// Method that checks if an exception derives from a certain type.
    /// </summary>
    /// <param name="e">The exception to check.</param>
    /// <param name="t">The type to check if the exception derives from.</param>
    /// <returns>True if the exception derives from the given type, false otherwise.</returns>
    private static bool DerivesFrom(Exception e, Type t)
    {
        return e.GetType().IsSubclassOf(t) || e.GetType() == t;
    }

    /// <summary>
    /// Method that converts an error message to a JSON string.
    /// </summary>
    /// <param name="message">The error message to convert.</param>
    /// <returns>The error message as a JSON string.</returns>
    private static string ToJson(string message)
    {
        return "{\"message\": \""+ message+"\"}";
    }

    /// <summary>
    /// Method that handles an exception related to authentication.
    /// </summary>
    /// <param name="e">The exception to handle.</param>
    public static AuthenticateResult HandleException(Exception e)
    {
        if(DerivesFrom(e, typeof(AuthenticationException)))
                return AuthenticationExceptionHandler.HandleAuthenticationException((AuthenticationException) e);

        

        Console.WriteLine(e.Message);
        return AuthenticateResult.Fail("Internal server error!");
        
    }

    /// <summary>
    /// Method that handles an exception related to authentication when the exception is of type AuthenticationException.
    /// </summary>
    /// <param name="e">The exception to handle.</param>
    /// <returns>An instance of AuthenticateResult representing the result of the authentication.</returns>
    private static AuthenticateResult HandleAuthenticationException(AuthenticationException e)
    {
        if(DerivesFrom(e,typeof(UserNotFoundException)))
        {
            return AuthenticateResult.Fail("Unauthorized!");
        }

        return AuthenticateResult.Fail("Internal server error!");

    }

}