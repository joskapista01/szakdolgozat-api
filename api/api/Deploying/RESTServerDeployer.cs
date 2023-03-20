namespace api.Deploying;

using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using api.Contracts.deployer;
using System.Text.Json;

public class RESTServerDeployer : IServerDeployer {

    private HttpClient client = new HttpClient();
    public RESTServerDeployer(string deployerAddress)
    {
        client.BaseAddress  = new Uri(deployerAddress);
    }

    public async Task<HttpStatusCode> CreateServer(string serverId, int serverPort)
    {
        CreateServerRequest body = new CreateServerRequest(serverId, serverPort);
        HttpResponseMessage response = await client.PostAsJsonAsync("", body);
        return response.StatusCode;
    }

    public async Task<HttpStatusCode> UpdateServer(string serverId, int instanceCount)
    {
        UpdateServerRequest request = new UpdateServerRequest(serverId, instanceCount);
        HttpResponseMessage response = await client.PutAsJsonAsync("", request);
        return response.StatusCode;
    }


    public async Task<HttpStatusCode> DeleteServer(string serverId)
    {
        DeleteServerRequest body = new DeleteServerRequest(serverId);
        var request = new HttpRequestMessage(HttpMethod.Delete, client.BaseAddress);
        request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        HttpResponseMessage response = await this.client.SendAsync(request);
        return response.StatusCode;
    }

}