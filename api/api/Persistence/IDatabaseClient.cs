using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Persistence
{
    public interface IDatabaseClient
    {
        public bool createServer(Server server);

        public bool deleteServer(string id, string user);

        public List<string> getServerList(string user);

        public Server getServerInfo(string id, string user);

        public User getUserCreds(string user);

        public bool addUser(string user, string password);

        public bool updateServerStatus(string user, string id, string status);

        public List<Server> getActiveServers();

    }
}
