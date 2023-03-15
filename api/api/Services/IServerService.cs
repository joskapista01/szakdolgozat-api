namespace api.Services;

using api.Contracts.api;

public interface IServerService
{
    public GetServerListResponse GetServerList(string user);
    public Task<GetServerResponse> GetServerInfo(string id,string user);
    public void CreateServer(CreateServerRequest request, string user);
    public void UpdateServer(string id, string user);
    public void DeleteServer(string id,string user);
}