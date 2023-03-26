using Microsoft.AspNetCore.Mvc;
using api.Contracts.api;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using System.Text;
using api.Exceptions;
using api.Handlers;
using api.Controllers.Helpers;

namespace api.Controllers;

[ApiController]
public class UserController : ControllerBase
{

    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }   


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

    [Authorize]
    [HttpPost("/users/login")]
    public IActionResult LoginUser()
    {
        return Ok(new LoginResponse(RequestHeaders.GetCurrentUser(Request)));
    } 
}

