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
    
    private int GetNextId()
    {
        for(int port = ServerConventions.START_PORT; port<=ServerConventions.END_PORT; port++)
        {
            if(!_databaseClient.IsPortAllocated(port))
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
                List<Server> activeServers = _databaseClient.getActiveServers();



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
                                UpdateServer(server.id, server.user);
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

    public GetServerListResponse GetServerList(string user)
    {
        List<String> serverIds = _databaseClient.getServerList(user);
        return new GetServerListResponse(serverIds);
    }
    public async Task<GetServerResponse> GetServerInfo(string id,string user)
    {
        Server serverInfo = _databaseClient.getServerInfo(id,user);
        ServerMonitorData serverMonitorData = await _serverMonitor.GetServerState(ServerConventions.GetServerHostname(id),ServerConventions.DEFAULT_SERVER_PORT); 

        return new GetServerResponse(serverInfo.id, serverInfo.serverName, serverMonitorData.playerCount, serverInfo.serverStatus, serverMonitorData.state, (serverInfo.serverUrl+":"+serverInfo.serverPort)); 

    }
    public async void CreateServer(CreateServerRequest request, string user)
    {   
        ServerCreation.ValidateServerName(request.name);     

        string serverId = GetNextId().ToString();
        string serverName = request.name;
        DateTime createdAt = DateTime.Now;
        string serverUrl = "servers.minecraft-hosting.io";
        int serverPort = int.Parse(serverId);
        string serverStatus = "ON";

        Server server = new Server(serverId, user, request.name, createdAt, serverUrl, serverPort, serverStatus);

        await _serverDeployer.CreateServer(serverId, serverPort);
        _databaseClient.createServer(server);
    }

    public async void UpdateServer(string id,string user)
    {
        Server serverInfo = _databaseClient.getServerInfo(id,user);
        if(serverInfo is null) return;

        (string new_status, int replicaCount) = serverInfo.serverStatus == "ON" ? ("OFF",0) : ("ON",1);

        await _serverDeployer.UpdateServer(id, replicaCount);
        _databaseClient.updateServerStatus(id,user,new_status);
    }
    public async void DeleteServer(string id,string user)
    {
        await _serverDeployer.DeleteServer(id);
        _databaseClient.deleteServer(id,user);
    }
}