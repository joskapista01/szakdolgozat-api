namespace api.Deploying;

using System.Net;

public interface IServerDeployer {
    
    public Task<HttpStatusCode> CreateServer(string serverId, int serverPort);

    public Task<HttpStatusCode> UpdateServer(string serverId, int instanceCount);

    public Task<HttpStatusCode> DeleteServer(string serverId);
}