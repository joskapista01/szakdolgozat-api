using api.Contracts.api;
using api.Persistence;
using api.Monitoring;
using api.Models;
using System.Net.Http.Headers;
using System.Web;
using api.Deploying;
using System.Diagnostics;
using api.Exceptions;
using api.Services.Helpers;
using System.Net;

namespace api.Services
{
    /// <summary>
    /// Service class responsible for managing servers.
    /// </summary>
    public class ServerService : IServerService
    {
        private readonly IDatabaseClient _databaseClient;
        private readonly IServerMonitor _serverMonitor;
        private readonly IServerDeployer _serverDeployer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerService"/> class.
        /// </summary>
        /// <param name="databaseClient">The database client.</param>
        /// <param name="serverMonitor">The server monitor.</param>
        /// <param name="serverDeployer">The server deployer.</param>
        public ServerService(IDatabaseClient databaseClient, IServerMonitor serverMonitor, IServerDeployer serverDeployer){
            _databaseClient = databaseClient;
            _serverMonitor = serverMonitor;
            _serverDeployer = serverDeployer;
            Task.Run(() => ServerHealthCheck());
        }
        
        /// <summary>
        /// Gets the next available server ID.
        /// </summary>
        /// <returns>The next available server ID.</returns>
        private async Task<int> GetNextId()
        {
            for(int port = ServerConventions.START_PORT; port<=ServerConventions.END_PORT; port++)
            {
                if(!await _databaseClient.IsPortAllocated(port))
                    return port;
            }
            throw new OutOfPortsException("Run out of free ports!");
        }

        /// <summary>
        /// Runs checks on servers periodically to manage running servers.
        /// </summary>
        private async void ServerHealthCheck() 
        {
            IDictionary<string, long>[] serverCatalog = {
                new Dictionary<string, long>(),
                new Dictionary<string, long>()
            };

            long currentTime = 0;

            while(true) 
            {
                try
                {
                    Thread.Sleep(ServerConventions.SERVER_CHECK_INTERVAL);
                    List<Server> activeServers = await _databaseClient.getActiveServers();

                    serverCatalog[1] = new Dictionary<string, long>();
                    foreach (Server server in activeServers)
                    {
                        ServerMonitorData serverInfo = await _serverMonitor.GetServerState(ServerConventions.GetServerHostname(server.id), ServerConventions.DEFAULT_SERVER_PORT);

                        if(serverInfo.playerCount == 0 && serverInfo.state == "online")
                        {
                            if(serverCatalog[0].ContainsKey(server.id)){
                                if(currentTime - serverCatalog[0][server.id] >= ServerConventions.SERVER_MAXIMUM_IDLE_TIME)
                                {
                                    await UpdateServer(server.id, server.user);
                                }
                                else
                                {
                                    serverCatalog[1][server.id] = serverCatalog[0][server.id];
                                }
                            }
                            else
                            {
                                serverCatalog[1][server.id] = currentTime;
                            }
                        }
                        

                    }
                    serverCatalog[0] = new Dictionary<string, long>(serverCatalog[1]);
                    currentTime+=ServerConventions.SERVER_CHECK_INTERVAL;
                } catch(Exception e)
                {
                    Console.WriteLine("[ERROR] Internal server error. ServerHealthCheck failed: " + e.Message);
                }
                
            }
        }

        /// <summary>
        /// Gets the list of servers belonging to the specified user.
        /// </summary>
        /// <param name="user">The username of the user whose servers are to be retrieved.</param>
        /// <returns>A <see cref="GetServerListResponse"/> object containing the list of server IDs.</returns>
        public async Task<GetServerListResponse> GetServerList(string user)
        {
            List<String> serverIds = await _databaseClient.getServerList(user);
            return new GetServerListResponse(serverIds);
        }

        /// <summary>
        /// Gets the information about the server with the specified ID belonging to the specified user.
        /// </summary>
        /// <param name="id">The ID of the server to retrieve.</param>
        /// <param name="user">The username of the user who owns the server.</param>
        /// <returns>A <see cref="GetServerResponse"/> object containing the server information.</returns>
        public async Task<GetServerResponse> GetServerInfo(string id,string user)
        {
            Server serverInfo = await _databaseClient.getServerInfo(id,user);

            ServerMonitorData serverMonitorData = await _serverMonitor.GetServerState(ServerConventions.GetServerHostname(id),ServerConventions.DEFAULT_SERVER_PORT); 

            return new GetServerResponse(serverInfo.id, serverInfo.serverName, serverMonitorData.playerCount, serverInfo.serverStatus, serverMonitorData.state, (serverInfo.serverUrl+":"+serverInfo.serverPort)); 

        }

        /// <summary>
        /// Creates a new Minecraft server with the specified name for the specified user.
        /// </summary>
        /// <param name="request">A <see cref="CreateServerRequest"/> object containing the server name.</param>
        /// <param name="user">The username of the user who will own the server.</param>
        /// <returns>An <see cref="HttpStatusCode"/> indicating the status of the operation.</returns>
        public async Task<HttpStatusCode> CreateServer(CreateServerRequest request, string user)
        {   
            ServerCreation.ValidateServerName(request.name);     

            string serverId = (await GetNextId()).ToString();
            string serverName = request.name;
            DateTime createdAt = DateTime.Now;
            string serverUrl = "servers.minecraft-hosting.io";
            int serverPort = int.Parse(serverId);
            string serverStatus = "ON";

            Server server = new Server(serverId, user, request.name, createdAt, serverUrl, serverPort, serverStatus);

            if(!await _databaseClient.createServer(server))
                return HttpStatusCode.InternalServerError;
            var response = await _serverDeployer.CreateServer(serverId, serverPort);
            return response; 
        }

        /// <summary>
        /// Updates the status of the server with the given ID for the specified user.
        /// </summary>
        /// <param name="id">The ID of the server to update.</param>
        /// <param name="user">The user who owns the server.</param>
        /// <returns>The HTTP status code indicating the result of the update.</returns>
        public async Task<HttpStatusCode> UpdateServer(string id,string user)
        {
            Server serverInfo = await _databaseClient.getServerInfo(id,user);

            (string new_status, int replicaCount) = serverInfo.serverStatus == "ON" ? ("OFF",0) : ("ON",1);

            if(!await _databaseClient.updateServerStatus(id,user,new_status))
                return HttpStatusCode.InternalServerError;
            var response = await _serverDeployer.UpdateServer(id, replicaCount);
            return response;
        }
        /// <summary>
        /// Deletes the server with the given ID for the specified user.
        /// </summary>
        /// <param name="id">The ID of the server to delete.</param>
        /// <param name="user">The user who owns the server.</param>
        /// <returns>The HTTP status code indicating the result of the deletion.</returns>
        public async Task<HttpStatusCode> DeleteServer(string id,string user)
        {
            if(!await _databaseClient.deleteServer(id,user))
                return HttpStatusCode.InternalServerError;
            var response = await _serverDeployer.DeleteServer(id);
            return response;
        }
    }
}