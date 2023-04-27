using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace api.Monitoring
{
    public class TestServerMonitor : IServerMonitor
    {
        public TestServerMonitor(){}
        public async Task<ServerMonitorData> GetServerState(string hostname, int port)
        {
            return new ServerMonitorData("offline", 5);
        }
    }
}