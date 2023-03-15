using Microsoft.AspNetCore.Mvc;
using api.Contracts.api;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using System.Text;

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
    public IActionResult RegisterUser(RegisterUserRequest request)
    {
        _userService.RegisterUser(request);
        
        return Ok(request);
    }

    [Authorize]
    [HttpPost("/users/login")]
    public IActionResult LoginUser()
    {
        return Ok(new LoginResponse(getCurrentUser()));
    } 

    private string getCurrentUser(){
        try {
            var headerValue = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            var bytes = Convert.FromBase64String(headerValue.Parameter);
            string credentials = Encoding.UTF8.GetString(bytes);
            string username = credentials.Split(":")[0];
            return username;
        } catch(Exception e){
            Console.WriteLine("Error, unauthorized request is trying to access username field!");
            return "";
        }
        
    }
}

