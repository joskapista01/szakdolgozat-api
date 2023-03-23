using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using api.Contracts.monitor;
using System.Text.Json;
using api.Exceptions;

namespace api.Monitoring
{
    public class RESTServerMonitor : IServerMonitor
    {
        private class ServerMonitorResponse
        {
            public bool alive { get; set; }
            public string? playerCount { get; set; }
        }
        private HttpClient client = new HttpClient();
        public RESTServerMonitor(string deployerAddress)
        {   
            client.BaseAddress  = new Uri(deployerAddress);
            client.Timeout = TimeSpan.FromMilliseconds(500);
        }
        
        public async Task<ServerMonitorData> GetServerState(string hostname, int port)
        {
            try
            {
                GetServerInfoRequest body = new GetServerInfoRequest(hostname, port);
                var request = new HttpRequestMessage(HttpMethod.Get, client.BaseAddress);
                request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await this.client.SendAsync(request);
                string responseBody = await response.Content.ReadAsStringAsync();
                var responseData = JsonSerializer.Deserialize<ServerMonitorResponse>(responseBody);

                string status = responseData.alive == true ? "online" : "offline";
                int playerCount = responseData.playerCount is not null ? int.Parse(responseData.playerCount) : 0;

                return new ServerMonitorData(status, playerCount);
            }
            catch(Exception e)
            {
                throw new MonitorException("[ERROR] Connection to the monitor failed: " + e.Message);
            }
        }
    }
}
