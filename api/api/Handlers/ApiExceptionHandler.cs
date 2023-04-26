using api.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Data.Common;

namespace api.Handlers
{
    /// <summary>
    /// Handles exceptions and returns appropriate HTTP status codes and response bodies.
    /// </summary>
    public class ApiExceptionHandler
    {
        /// <summary>
        /// Returns true if the given exception is a subclass of the given type or the same type.
        /// </summary>
        private static bool DerivesFrom(Exception e, Type t)
        {
            return e.GetType().IsSubclassOf(t) || e.GetType() == t;
        }

        /// <summary>
        /// Returns the given message as a JSON string.
        /// </summary>
        private static string ToJson(string message)
        {
            return "{\"message\": \""+ message+"\"}";
        }

        /// <summary>
        /// Handles the given exception and returns an IActionResult representing the appropriate HTTP response.
        /// </summary>
        /// <param name="e">The exception to handle.</param>
        /// <returns>An IActionResult representing the appropriate HTTP response.</returns>
        public static IActionResult HandleException(Exception e)
        {
            if(DerivesFrom(e, typeof(ApiException)))
                    return ApiExceptionHandler.HandleApiException((ApiException) e);
            
            if(DerivesFrom(e, typeof(DbException)) && e.InnerException is not null)
                return ApiExceptionHandler.HandleApiException((ApiException) e.InnerException);

            

            Console.WriteLine(e.Message);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            
        }

        /// <summary>
        /// Handles the given ApiException and returns an IActionResult representing the appropriate HTTP response.
        /// </summary>
        /// <param name="e">The ApiException to handle.</param>
        /// <returns>An IActionResult representing the appropriate HTTP response.</returns>
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

        /// <summary>
        /// Handles the given CreateServerException and returns an IActionResult representing the appropriate HTTP response.
        /// </summary>
        /// <param name="e">The CreateServerException to handle.</param>
        /// <returns>An IActionResult representing the appropriate HTTP response.</returns>
        private static IActionResult HandleCreateServerExceptions(CreateServerException e)
        {
            if(DerivesFrom(e,typeof(InvalidServerNameException)))
            {
                return new BadRequestObjectResult(ToJson(e.Message));
            }

            Console.WriteLine(e.Message);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError); 

        }

        /// <summary>
        /// Handles exceptions that occur during user registration.
        /// </summary>
        /// <param name="e">The exception to handle.</param>
        /// <returns>An IActionResult representing the appropriate response for the exception.</returns>
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
            else if(DerivesFrom(e,typeof(UsernameAlreadyTakenException)))
            {
                return new BadRequestObjectResult(ToJson(e.Message));
            }  

            Console.WriteLine(e.Message);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        /// <summary>
        /// Handles exceptions that occur while interacting with the database.
        /// </summary>
        /// <param name="e">The exception to handle.</param>
        /// <returns>An IActionResult representing the appropriate response for the exception.</returns>
        private static IActionResult HandleDatabaseException(DatabaseException e)
        {

            if(DerivesFrom(e, typeof(ServerNotFoundException)))
            {
                return new NotFoundObjectResult(ToJson(e.Message));
            } else if(DerivesFrom(e, typeof(UserNotFoundException)))
            {
                return new UnauthorizedObjectResult(ToJson(e.Message));
            }

            Console.WriteLine(e.Message);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError); 
        }

    /// <summary>
    /// Handles exceptions thrown by the Monitor.
    /// </summary>
    /// <param name="e">The MonitorException that was thrown.</param>
    /// <returns>An IActionResult representing the response to the exception.</returns>
        private static IActionResult HandleMonitorException(MonitorException e)
        {
            Console.WriteLine(e.Message);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError); 
        }

        /// <summary>
        /// Handles exceptions thrown by the Deployer.
        /// </summary>
        /// <param name="e">The DeployerException that was thrown.</param>
        /// <returns>An IActionResult representing the response to the exception.</returns>
        private static IActionResult HandleDeployerException(DeployerException e)
        {
            Console.WriteLine(e.Message);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError); 
        }

        /// <summary>
        /// Handles exceptions thrown when there are no available ports.
        /// </summary>
        /// <param name="e">The OutOfPortsException that was thrown.</param>
        /// <returns>An IActionResult representing the response to the exception.</returns>
        private static IActionResult HandlOutOfPortsException(OutOfPortsException e)
        {
            Console.WriteLine(e.Message);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError); 
        }
    }
}