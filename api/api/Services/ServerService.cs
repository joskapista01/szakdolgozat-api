namespace api.Services;

using api.Contracts.api;
using api.Persistence;
using api.Monitoring;
using api.Models;
using System.Net.Http.Headers;
using System.Web;
using api.Deploying;
using System.Diagnostics;

public class ServerService : IServerService
{

    private int id;

    private const int SERVER_CHECK_INTERVAL = 10000; //milisec
    private const int SERVER_MAXIMUM_IDLE_TIME = 60000; //millisec

    private string GetServerHostname(string id) 
    {
        return "minecraft-server-"+id+".servers.svc.cluster.local";
    } 

    private const int DEFAULT_SERVER_PORT = 25565;

    private readonly IDatabaseClient _databaseClient;
    private readonly IServerMonitor _serverMonitor;
    private readonly IServerDeployer _serverDeployer;
    public ServerService(IDatabaseClient databaseClient, IServerMonitor serverMonitor, IServerDeployer serverDeployer){
        _databaseClient = databaseClient;
        _serverMonitor = serverMonitor;
        _serverDeployer = serverDeployer;
        id = 20000;
        Task.Run(() => ServerHealthCheck());
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
            Thread.Sleep(SERVER_CHECK_INTERVAL);
            List<Server> activeServers = _databaseClient.getActiveServers();



            serverCatalog[1] = new Dictionary<string, long>();
            foreach (Server server in activeServers)
            {
                ServerMonitorData serverInfo = await _serverMonitor.GetServerState(GetServerHostname(server.id), DEFAULT_SERVER_PORT);

                if(serverInfo.playerCount == 0 && serverInfo.state == "online")
                {
                    if(serverCatalog[0].ContainsKey(server.id)){
                        Console.WriteLine(currentTime);
                        Console.WriteLine("contains " + (currentTime - serverCatalog[0][server.id]));
                        if(currentTime - serverCatalog[0][server.id] >= SERVER_MAXIMUM_IDLE_TIME)
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
            currentTime+=SERVER_CHECK_INTERVAL;
            
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
        ServerMonitorData serverMonitorData = await _serverMonitor.GetServerState(GetServerHostname(id), DEFAULT_SERVER_PORT); 

        if(serverInfo is not null){
            return new GetServerResponse(serverInfo.id, serverInfo.serverName, serverMonitorData.playerCount, serverInfo.serverStatus, serverMonitorData.state, (serverInfo.serverUrl+":"+serverInfo.serverPort)); 
        }
        return null;
    }
    public void CreateServer(CreateServerRequest request, string user)
    {        
        string serverId = id.ToString();
        string serverName = request.name;
        DateTime createdAt = DateTime.Now;
        string serverUrl = "servers.minecraft-hosting.io";
        int serverPort = id;
        string serverStatus = "ON";

        Server server = new Server(serverId, user, request.name, createdAt, serverUrl, serverPort, serverStatus);

        _serverDeployer.CreateServer(serverId, serverPort);
        _databaseClient.createServer(server);
        id++;
    }

    public void UpdateServer(string id,string user)
    {
        Server serverInfo = _databaseClient.getServerInfo(id,user);
        if(serverInfo is null) return;

        (string new_status, int replicaCount) = serverInfo.serverStatus == "ON" ? ("OFF",0) : ("ON",1);

        _serverDeployer.UpdateServer(id, replicaCount);
        _databaseClient.updateServerStatus(id,user,new_status);
    }
    public void DeleteServer(string id,string user)
    {
        _serverDeployer.DeleteServer(id);
        _databaseClient.deleteServer(id,user);
    }

}