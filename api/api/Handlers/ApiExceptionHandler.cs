using api.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace api.Handlers;

public class ApiExceptionHandler
{

    private static bool DerivesFrom(Exception e, Type t)
    {
        return e.GetType().IsSubclassOf(t) || e.GetType() == t;
    }

    private static string ToJson(string message)
    {
        return "{\"message:\": \""+ message+"\"}";
    }

    public static IActionResult HandleException(Exception e)
    {
        if(DerivesFrom(e, typeof(ApiException)))
                return ApiExceptionHandler.HandleApiException((ApiException) e);

        Console.WriteLine(e.Message);
        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        
    }

    private static IActionResult HandleApiException(ApiException e)
    {
        if(DerivesFrom(e,typeof(CreateServerException)))
        {
            return HandleCreateServerExceptions((CreateServerException) e);
        }
        else if(DerivesFrom(e,typeof(RegisterUserException)))
        {
            return HandleRegisterUserExceptions((RegisterUserException) e);
        }
        else if(DerivesFrom(e,typeof(DatabaseException)))
        {
            return HandleDatabaseException((DatabaseException) e);
        }
        else if(DerivesFrom(e,typeof(MonitorException)))
        {
            return HandleMonitorException((MonitorException) e);
        }
        else if(DerivesFrom(e,typeof(DeployerException)))
        {
            return HandleDeployerException((DeployerException) e);
        }
        else if(DerivesFrom(e,typeof(OutOfPortsException)))
        {
            return HandlOutOfPortsException((OutOfPortsException) e);
        }

        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }

    private static IActionResult HandleCreateServerExceptions(CreateServerException e)
    {
        if(DerivesFrom(e,typeof(InvalidServerNameException)))
        {
            return new BadRequestObjectResult(ToJson(e.Message));
        }

        Console.WriteLine(e.Message);
        return new StatusCodeResult(StatusCodes.Status500InternalServerError); 

    }

    private static IActionResult HandleRegisterUserExceptions(RegisterUserException e)
    {
        if(DerivesFrom(e,typeof(InvalidUsernameException)))
        {
            return new BadRequestObjectResult(ToJson(e.Message));
        } 
        else if (DerivesFrom(e,typeof(InvalidPasswordException)))
        {
            return new BadRequestObjectResult(ToJson(e.Message));
        } 

        Console.WriteLine(e.Message);
        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }

    private static IActionResult HandleDatabaseException(DatabaseException e)
    {
        Type exceptionType = e.GetType();

        if(exceptionType == typeof(UsernameAlreadyTakenException))
        {
            return new BadRequestObjectResult(ToJson(e.Message));
        } 

        Console.WriteLine(e.Message);
        return new StatusCodeResult(StatusCodes.Status500InternalServerError); 
    }

    private static IActionResult HandleMonitorException(MonitorException e)
    {
        Console.WriteLine(e.Message);
        return new StatusCodeResult(StatusCodes.Status500InternalServerError); 
    }

    private static IActionResult HandleDeployerException(DeployerException e)
    {
        Console.WriteLine(e.Message);
        return new StatusCodeResult(StatusCodes.Status500InternalServerError); 
    }

    private static IActionResult HandlOutOfPortsException(OutOfPortsException e)
    {
        Console.WriteLine(e.Message);
        return new StatusCodeResult(StatusCodes.Status500InternalServerError); 
    }
}