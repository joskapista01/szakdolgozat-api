namespace api.Services;

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

public class ServerService : IServerService
{
    private readonly IDatabaseClient _databaseClient;
    private readonly IServerMonitor _serverMonitor;
    private readonly IServerDeployer _serverDeployer;
    public ServerService(IDatabaseClient databaseClient, IServerMonitor serverMonitor, IServerDeployer serverDeployer){
        _databaseClient = databaseClient;
        _serverMonitor = serverMonitor;
        _serverDeployer = serverDeployer;
        Task.Run(() => ServerHealthCheck());
    }
    
    private async Task<int> GetNextId()
    {
        for(int port = ServerConventions.START_PORT; port<=ServerConventions.END_PORT; port++)
        {
            if(!await _databaseClient.IsPortAllocated(port))
                return port;
        }
        throw new OutOfPortsException("Run out of free ports!");
    }

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
                            Console.WriteLine(currentTime);
                            Console.WriteLine("contains " + (currentTime - serverCatalog[0][server.id]));
                            if(currentTime - serverCatalog[0][server.id] >= ServerConventions.SERVER_MAXIMUM_IDLE_TIME)
                            {
                                Console.WriteLine("goof");
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

    public async Task<GetServerListResponse> GetServerList(string user)
    {
        List<String> serverIds = await _databaseClient.getServerList(user);
        return new GetServerListResponse(serverIds);
    }
    public async Task<GetServerResponse> GetServerInfo(string id,string user)
    {
        Server serverInfo = await _databaseClient.getServerInfo(id,user);

        ServerMonitorData serverMonitorData = await _serverMonitor.GetServerState(ServerConventions.GetServerHostname(id),ServerConventions.DEFAULT_SERVER_PORT); 

        return new GetServerResponse(serverInfo.id, serverInfo.serverName, serverMonitorData.playerCount, serverInfo.serverStatus, serverMonitorData.state, (serverInfo.serverUrl+":"+serverInfo.serverPort)); 

    }
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

        var response = await _serverDeployer.CreateServer(serverId, serverPort);
        if(await _databaseClient.createServer(server))
            return response;
        else return HttpStatusCode.InternalServerError;
    }

    public async Task<HttpStatusCode> UpdateServer(string id,string user)
    {
        Server serverInfo = await _databaseClient.getServerInfo(id,user);

        (string new_status, int replicaCount) = serverInfo.serverStatus == "ON" ? ("OFF",0) : ("ON",1);

        var response = await _serverDeployer.UpdateServer(id, replicaCount);
        if(await _databaseClient.updateServerStatus(id,user,new_status))
            return response;
        else return HttpStatusCode.InternalServerError;
    }
    public async Task<HttpStatusCode> DeleteServer(string id,string user)
    {
        var response = await _serverDeployer.DeleteServer(id);
        if(await _databaseClient.deleteServer(id,user))
            return response;
        else return HttpStatusCode.InternalServerError;
    }
}