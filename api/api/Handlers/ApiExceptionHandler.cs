using api.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace api.Handlers;

public class ApiExceptionHandler
{
    public static IActionResult HandleException(Exception e)
    {
        if(e.GetType().IsSubclassOf(typeof(ApiException)))
                return ApiExceptionHandler.HandleApiException((ApiException) e);

        Console.WriteLine(e.Message);
        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        
    }

    private static IActionResult HandleApiException(ApiException e)
    {
        Type exceptionType = e.GetType();

        if(exceptionType.IsSubclassOf(typeof(CreateServerException)))
        {
            return HandleCreateServerExceptions((CreateServerException) e);
        }
        else if(exceptionType.IsSubclassOf(typeof(RegisterUserException)))
        {
            return HandleRegisterUserExceptions((RegisterUserException) e);
        }
        else if(exceptionType.IsSubclassOf(typeof(DatabaseException)))
        {
            return HandleDatabaseException((DatabaseException) e);
        }
        else if(exceptionType.IsSubclassOf(typeof(MonitorException)))
        {
            return HandleMonitorException((MonitorException) e);
        }
        else if(exceptionType.IsSubclassOf(typeof(DeployerException)))
        {
            return HandleDeployerException((DeployerException) e);
        }
        else if(exceptionType.IsSubclassOf(typeof(OutOfPortsException)))
        {
            return HandlOutOfPortsException((OutOfPortsException) e);
        }

        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }

    private static IActionResult HandleCreateServerExceptions(CreateServerException e)
    {
        Type exceptionType = e.GetType();

        if(exceptionType == typeof(InvalidServerNameException))
        {
            return new BadRequestObjectResult(e.Message);
        }

        Console.WriteLine(e.Message);
        return new StatusCodeResult(StatusCodes.Status500InternalServerError); 

    }

    private static IActionResult HandleRegisterUserExceptions(RegisterUserException e)
    {
        Type exceptionType = e.GetType();

        if(exceptionType == typeof(InvalidUsernameException))
        {
            return new BadRequestObjectResult(e.Message);
        } 
        else if (exceptionType == typeof(InvalidPasswordException))
        {
            return new BadRequestObjectResult(e.Message);
        } 

        Console.WriteLine(e.Message);
        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }

    private static IActionResult HandleDatabaseException(DatabaseException e)
    {
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