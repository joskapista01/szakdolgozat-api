namespace api.Services;

using api.Contracts.api;
using System.Net;

public interface IServerService
{
    public GetServerListResponse GetServerList(string user);
    public Task<GetServerResponse> GetServerInfo(string id,string user);
    public Task<HttpStatusCode> CreateServer(CreateServerRequest request, string user);
    public Task<HttpStatusCode> UpdateServer(string id, string user);
    public Task<HttpStatusCode> DeleteServer(string id,string user);
}