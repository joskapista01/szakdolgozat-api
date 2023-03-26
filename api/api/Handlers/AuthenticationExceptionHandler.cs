using api.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Data.Common;
using Microsoft.AspNetCore.Authentication;

namespace api.Handlers;

public class AuthenticationExceptionHandler
{
    private static bool DerivesFrom(Exception e, Type t)
    {
        return e.GetType().IsSubclassOf(t) || e.GetType() == t;
    }

    private static string ToJson(string message)
    {
        return "{\"message\": \""+ message+"\"}";
    }

    public static AuthenticateResult HandleException(Exception e)
    {
        if(DerivesFrom(e, typeof(AuthenticationException)))
                return AuthenticationExceptionHandler.HandleAuthenticationException((AuthenticationException) e);

        

        Console.WriteLine(e.Message);
        return AuthenticateResult.Fail("Internal server error!");
        
    }

    private static AuthenticateResult HandleAuthenticationException(AuthenticationException e)
    {
        if(DerivesFrom(e,typeof(UserNotFoundException)))
        {
            return AuthenticateResult.Fail("Unauthorized!");
        }

        return AuthenticateResult.Fail("Internal server error!");

    }

}