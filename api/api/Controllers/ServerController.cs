using Microsoft.AspNetCore.Mvc;
using api.Contracts.api;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using System.Text;
using api.Handlers;
using api.Controllers.Helpers;

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
    public async Task<IActionResult> GetServerList()
    {
        try 
        {
            string user = RequestHeaders.GetCurrentUser(Request);
            var response = await _serverService.GetServerList(user);
            return Ok(response);
        }
        catch (Exception e)
        {
            return ApiExceptionHandler.HandleException(e);
        }
        
    }

    [HttpGet("/server/{id}")]
    public async Task<IActionResult> GetServer(string id)
    {
        try
        {
            string user = RequestHeaders.GetCurrentUser(Request);

            var response = await _serverService.GetServerInfo(id, user);

            if(response is not null)
                return Ok(response);
            else 
                return NotFound(id);
        }
        catch (Exception e)
        {
            return ApiExceptionHandler.HandleException(e);
        }
    }
    [HttpPost("/server")]
    public async Task<IActionResult> CreateServer(CreateServerRequest request)
    {
        try
        {
            string user = RequestHeaders.GetCurrentUser(Request);

            await _serverService.CreateServer(request, user);

            return Ok(request);
        }
        catch (Exception e)
        {
            return ApiExceptionHandler.HandleException(e);
        }
    }

    [HttpPut("/server/{id}")]
    public IActionResult UpdateServer(string id)
    {
        try
        {
            string user = RequestHeaders.GetCurrentUser(Request);

            _serverService.UpdateServer(id, user);

            return Ok(id);
        }
        catch (Exception e)
        {
            return ApiExceptionHandler.HandleException(e);
        }
    }

    [HttpDelete("/server/{id}")]
    public IActionResult DeleteServer(string id) {
        try
        {
            string user = RequestHeaders.GetCurrentUser(Request);

            _serverService.DeleteServer(id, user);

            return Ok(id);
        }
        catch (Exception e)
        {
            return ApiExceptionHandler.HandleException(e);
        }
    }
}

