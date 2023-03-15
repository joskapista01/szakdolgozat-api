using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Persistence
{
    public class TestDatabaseClient : IDatabaseClient{

        private List<Server> servers;
        private List<User> users;
        public TestDatabaseClient() {
            servers = new List<Server>();
            users = new List<User>();
            users.Add(new User("test", "test"));
            users.Add(new User("test1", "test1"));
        }

        public bool createServer(Server server)
        {
            lock(servers){
            servers.Add(server);
            }
            return true;
        }

        public bool deleteServer(string id, string user)
        {
            lock(servers){
                servers.RemoveAll(e => e.id == id && e.user == user);
            }
            return true;
        }

        public List<string> getServerList(string user)
        {
            lock(servers)
            {
                List<string> serverIds = servers.FindAll(e => e.user == user).Select(e => e.id).ToList();
                return serverIds;
            }
        }

        public Server getServerInfo(string id, string user)
        {
            lock(servers){
                Server server = servers.FirstOrDefault(e => e.id == id && e.user == user);
                return server;  
            }
            
            
        }

        public User getUserCreds(string username)
        {
            lock(users){
                var user = users.FirstOrDefault(e => e.username == username);       
                return user; 
            }
            
        }

        public bool addUser(string username, string password)
        {
            lock(users)
            {
                if(users.FirstOrDefault(e => e.username == username) != null)
                    return false;

                User user = new User(username, password);
                users.Add(user);
            }

            return true;
        }

        public bool updateServerStatus(string id, string user, string status){

            lock(servers)
            {
                int index = servers.FindIndex(e => e.id == id && e.user == user);
                if(index==-1)
                    return false;
                Server server = servers[index];
                servers[index] = new Server(server.id, server.user, server.serverName, server.createdAt, server.serverUrl, server.serverPort, status);
            }
            return true;
        }

        public List<Server> getActiveServers()
        {
            lock(servers)
            {
                List<Server> result = servers.FindAll(s => s.serverStatus == "ON").ToList();
                return result;
            }
        }
    }
}