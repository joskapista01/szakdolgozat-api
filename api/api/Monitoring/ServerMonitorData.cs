using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Monitoring
{
    public record ServerMonitorData(string state, int playerCount);
}
