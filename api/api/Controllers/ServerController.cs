using Microsoft.AspNetCore.Mvc;
using api.Contracts.api;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using System.Text;

namespace api.Controllers;

[ApiController]
[Authorize]
public class ServerController : ControllerBase
{

    private readonly IServerService _serverService;

    public ServerController(IServerService serverService)
    {
        _serverService = serverService;
    }   

    [HttpGet("/servers")]
    public IActionResult GetServerList()
    {

        string user = getCurrentUser();

        var response = _serverService.GetServerList(user);
        return Ok(response);
    }

    [HttpGet("/server/{id}")]
    public async Task<IActionResult> GetServer(string id)
    {
        string user = getCurrentUser();        

        var response = await _serverService.GetServerInfo(id, user);

        if(response is not null)
            return Ok(response);
        else 
            return NotFound(id);
    }
    [HttpPost("/server")]
    public IActionResult CreateServer(CreateServerRequest request)
    {
        string user = getCurrentUser();


        _serverService.CreateServer(request, user);
        return Ok(request);
    }

    [HttpPut("/server/{id}")]
    public IActionResult UpdateServer(string id)
    {
        string user = getCurrentUser();

        _serverService.UpdateServer(id, user);
        return Ok(id);
    }

    [HttpDelete("/server/{id}")]
    public IActionResult DeleteServer(string id) {
        string user = getCurrentUser();

        _serverService.DeleteServer(id, user); 
        return Ok(id);
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

