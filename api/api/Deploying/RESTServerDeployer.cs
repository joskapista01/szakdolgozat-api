using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using api.Contracts.deployer;
using System.Text.Json;
using api.Exceptions;

namespace api.Deploying
{
    /// <summary>
    /// A REST-based implementation of the IServerDeployer interface.
    /// </summary>
    public class RESTServerDeployer : IServerDeployer {

        private HttpClient client = new HttpClient();

        /// <summary>
        /// Initializes a new instance of the <see cref="RESTServerDeployer"/> class.
        /// </summary>
        /// <param name="deployerAddress">The address of the server deployer.</param>
        public RESTServerDeployer(string deployerAddress)
        {
            client.BaseAddress  = new Uri(deployerAddress);
            client.Timeout = TimeSpan.FromMilliseconds(2000);
        }

        /// <summary>
        /// Sends a request to the server deployer to create a new server instance.
        /// </summary>
        /// <param name="serverId">The ID of the server to create.</param>
        /// <param name="serverPort">The port number the new server instance should use.</param>
        /// <returns>The HTTP status code of the response.</returns>
        public async Task<HttpStatusCode> CreateServer(string serverId, int serverPort)
        {
            try
            { 
                CreateServerRequest body = new CreateServerRequest(serverId, serverPort);
                HttpResponseMessage response = await client.PostAsJsonAsync("", body);
                return response.StatusCode;
            } 
            catch(Exception e)
            {
                throw new DeployerException("[ERROR] Connection to the deployer failed: " + e.Message);
            }
        }

        /// <summary>
        /// Sends a request to the server deployer to update the instance count of a server.
        /// </summary>
        /// <param name="serverId">The ID of the server to update.</param>
        /// <param name="instanceCount">The new number of instances for the server.</param>
        /// <returns>The HTTP status code of the response.</returns>
        public async Task<HttpStatusCode> UpdateServer(string serverId, int instanceCount)
        {
            try
            {
                UpdateServerRequest request = new UpdateServerRequest(serverId, instanceCount);
                HttpResponseMessage response = await client.PutAsJsonAsync("", request);
                return response.StatusCode;
            }
            catch(Exception e)
            {
                throw new DeployerException("[ERROR] Connection to the deployer failed: " + e.Message);
            }
        }

        /// <summary>
        /// Sends a request to the server deployer to delete a server instance.
        /// </summary>
        /// <param name="serverId">The ID of the server to delete.</param>
        /// <returns>The HTTP status code of the response.</returns>
        public async Task<HttpStatusCode> DeleteServer(string serverId)
        {
            try
            {
                DeleteServerRequest body = new DeleteServerRequest(serverId);
                var request = new HttpRequestMessage(HttpMethod.Delete, client.BaseAddress);
                request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await this.client.SendAsync(request);
                return response.StatusCode;
            }
            catch(Exception e)
            {
                throw new DeployerException("[ERROR] Connection to the deployer failed: " + e.Message);
            }

        }

    }
}