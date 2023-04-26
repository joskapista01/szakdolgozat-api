using Microsoft.AspNetCore.Mvc;
using api.Contracts.api;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using System.Text;
using api.Exceptions;
using api.Handlers;
using api.Controllers.Helpers;

namespace api.Controllers
{
    /// <summary>
    /// Controller for managing user operations.
    /// </summary>
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;

        /// <summary>
        /// Creates a new instance of the UserController class.
        /// </summary>
        /// <param name="userService">The service for managing users.</param>
        public UserController(IUserService userService)
        {
            _userService = userService;
        }   


        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="request">The request object containing user registration details.</param>
        /// <returns>The result of the user registration operation.</returns>
        [HttpPost("/users/register")]
        public async Task<IActionResult> RegisterUser(RegisterUserRequest request)
        {
            try {
                var result = await _userService.RegisterUser(request);
                return Ok();
            }
            catch (Exception e)
            {
                return ApiExceptionHandler.HandleException(e);
            }
            
            
        }

        /// <summary>
        /// Logs in a user.
        /// </summary>
        /// <returns>The result of the user login operation.</returns>
        [Authorize]
        [HttpPost("/users/login")]
        public IActionResult LoginUser()
        {
            return Ok(new LoginResponse(RequestHeaders.GetCurrentUser(Request)));
        } 
    }
}