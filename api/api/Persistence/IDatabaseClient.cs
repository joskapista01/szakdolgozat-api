using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Persistence
{
    public interface IDatabaseClient
    {
        public Task<bool> createServer(Server server);

        public Task<bool> deleteServer(string id, string user);

        public Task<List<string>> getServerList(string user);

        public Task<Server> getServerInfo(string id, string user);

        public Task<User> getUserCreds(string user);

        public Task<bool> addUser(User user);

        public Task<bool> updateServerStatus(string user, string id, string status);

        public Task<List<Server>> getActiveServers();

        public Task<bool> IsPortAllocated(int port);

    }
}
