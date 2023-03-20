using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Monitoring
{
    public interface IServerMonitor
    {
        public Task<ServerMonitorData> GetServerState(string hostname, int port);
    }
}
