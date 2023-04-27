using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Exceptions;

namespace api.Persistence
{
    public class TestDatabaseClient : IDatabaseClient{

        private List<Server> servers;
        private List<User> users;
        public TestDatabaseClient() {
            servers = new List<Server>();
            users = new List<User>();
            users.Add(new User("testuser", "Test1234"));
            users.Add(new User("testuserWithServer", "Test1234"));
            servers.Add(new Server("1", "testuserWithServer", "test", DateTime.Now, "something.somewhere.io", 10000, "OFF"));
        }

        public async Task<bool> createServer(Server server)
        {
            lock(servers){
            servers.Add(server);
            }
            return true;
        }

        public async Task<bool> deleteServer(string id, string user)
        {
            lock(servers){
                servers.RemoveAll(e => e.id == id && e.user == user);
            }
            return true;
        }

        public async Task<List<string>> getServerList(string user)
        {
            lock(servers)
            {
                List<string> serverIds = servers.FindAll(e => e.user == user).Select(e => e.id).ToList();
                return serverIds;
            }
        }

        public async Task<Server> getServerInfo(string id, string user)
        {
            lock(servers){
                Server server = servers.FirstOrDefault(e => e.id == id && e.user == user);
                return server;  
            }
            
            
        }

        public async Task<User> getUserCreds(string username)
        {
            lock(users){
                var user = users.FirstOrDefault(e => e.username == username);       
                return user; 
            }
            
        }

        public async Task<bool> addUser(User user)
        {
            lock(users)
            {
                if(users.FirstOrDefault(e => e.username == user.username) != null)
                    throw new UsernameAlreadyTakenException("Username already taken!");

                users.Add(user);
            }
            return true;

        }

        public async Task<bool> updateServerStatus(string id, string user, string status){

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

        public async Task<List<Server>> getActiveServers()
        {
            lock(servers)
            {
                List<Server> result = servers.FindAll(s => s.serverStatus == "ON").ToList();
                return result;
            }
        }

        public async Task<bool> IsPortAllocated(int port)
        {
            lock(servers)
            {
                return servers.FindIndex(e => e.serverPort==port) >= 0;
            }
        }
    }
}