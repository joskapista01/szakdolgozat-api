using Microsoft.AspNetCore.Mvc;
using api.Contracts.api;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using System.Text;
using api.Handlers;
using api.Controllers.Helpers;

namespace api.Controllers
{
    /// <summary>
    /// Controller for managing server operations.
    /// </summary>
    [ApiController]
    [Authorize]
    public class ServerController : ControllerBase
    {

        private readonly IServerService _serverService;

        /// <summary>
        /// Creates a new instance of the ServerController class.
        /// </summary>
        /// <param name="serverService">The service for managing servers.</param>
        public ServerController(IServerService serverService)
        {
            _serverService = serverService;
        }   

        /// <summary>
        /// Retrieves a list of servers belonging to the current user.
        /// </summary>
        /// <returns>The list of servers belonging to the current user.</returns>
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

        /// <summary>
        /// Retrieves the details of a specific server belonging to the current user.
        /// </summary>
        /// <param name="id">The ID of the server to retrieve.</param>
        /// <returns>The details of the specified server.</returns>
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

        /// <summary>
        /// Creates a new server for the current user.
        /// </summary>
        /// <param name="request">The request object containing the details of the new server.</param>
        /// <returns>The result of the server creation operation.</returns>
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

        /// <summary>
        /// Updates the details of a specific server belonging to the current user.
        /// </summary>
        /// <param name="id">The ID of the server to update.</param>
        /// <returns>The result of the server update operation.</returns>
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

        /// <summary>
        /// Deletes a specific server belonging to the current user.
        /// </summary>
        /// <param name="id">The ID of the server to delete.</param>
        /// <returns>The result of the server deletion operation.</returns>
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

}