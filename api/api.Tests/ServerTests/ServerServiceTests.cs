using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Services;
using api.Persistence;
using api.Monitoring;
using api.Deploying;
using api.Contracts;
using api.Contracts.api;
using System.Net;
using api.Exceptions;

namespace api.Tests.ServerTests
{
    [TestFixture]
    internal class ServerServiceTests
    {
        [Test]
        public async Task GetServerListForUserWithNoServers()
        {
            IDatabaseClient _dbClient = new TestDatabaseClient();
            IServerDeployer _serverDeployer = new TestServerDeployer();
            IServerMonitor _serverMonitor = new TestServerMonitor();
            IServerService _serverService = new ServerService(_dbClient, _serverMonitor, _serverDeployer);
            GetServerListResponse result = await _serverService.GetServerList("testuser");
            Assert.That(result.serverIds, Is.EqualTo(new List<string>()));
        }

        [Test]
        public async Task GetServerListForUserWithServers()
        {
            IDatabaseClient _dbClient = new TestDatabaseClient();
            IServerDeployer _serverDeployer = new TestServerDeployer();
            IServerMonitor _serverMonitor = new TestServerMonitor();
            IServerService _serverService = new ServerService(_dbClient, _serverMonitor, _serverDeployer);
            GetServerListResponse result = await _serverService.GetServerList("testuserWithServer");
            Assert.That(result.serverIds, Is.EqualTo(new List<string>() { "1" }));
        }

        [Test]
        public async Task GetServerInfo()
        {
            IDatabaseClient _dbClient = new TestDatabaseClient();
            IServerDeployer _serverDeployer = new TestServerDeployer();
            IServerMonitor _serverMonitor = new TestServerMonitor();
            IServerService _serverService = new ServerService(_dbClient, _serverMonitor, _serverDeployer);
            GetServerResponse result = await _serverService.GetServerInfo("1", "testuserWithServer");
            Assert.That(result, Is.EqualTo(new GetServerResponse("1", "test", 5, "OFF", "offline", "something.somewhere.io:10000")));
        }

        [Test]
        public async Task CreateServerWithValidServerName()
        {
            IDatabaseClient _dbClient = new TestDatabaseClient();
            IServerDeployer _serverDeployer = new TestServerDeployer();
            IServerMonitor _serverMonitor = new TestServerMonitor();
            IServerService _serverService = new ServerService(_dbClient, _serverMonitor, _serverDeployer);
            CreateServerRequest request = new CreateServerRequest("test");
            HttpStatusCode result = await _serverService.CreateServer(request, "testuser");
            Assert.That(result, Is.EqualTo(HttpStatusCode.OK));

            GetServerResponse serverInfo = await _serverService.GetServerInfo("20000", "testuser");
            Assert.That(serverInfo, Is.EqualTo(new GetServerResponse("20000", "test", 5, "ON", "offline", "servers.minecraft-hosting.io:20000")));


        }

        [Test]
        public async Task CreateServerWithInvalidServerName()
        {
            IDatabaseClient _dbClient = new TestDatabaseClient();
            IServerDeployer _serverDeployer = new TestServerDeployer();
            IServerMonitor _serverMonitor = new TestServerMonitor();
            IServerService _serverService = new ServerService(_dbClient, _serverMonitor, _serverDeployer);
            CreateServerRequest request = new CreateServerRequest("");
            try
            {
               HttpStatusCode result = await _serverService.CreateServer(request, "testuser");
            }
            catch(Exception e)
            {
                Assert.That(e.GetType(), Is.EqualTo(typeof(InvalidServerNameException)));
            }
        }

        [Test]
        public async Task UpdateServer()
        {
            IDatabaseClient _dbClient = new TestDatabaseClient();
            IServerDeployer _serverDeployer = new TestServerDeployer();
            IServerMonitor _serverMonitor = new TestServerMonitor();
            IServerService _serverService = new ServerService(_dbClient, _serverMonitor, _serverDeployer);

            HttpStatusCode result = await _serverService.UpdateServer("1", "testuserWithServer");
            Assert.That(result, Is.EqualTo(HttpStatusCode.OK));
            GetServerResponse serverInfo = await _serverService.GetServerInfo("1", "testuserWithServer");
            Assert.That(serverInfo, Is.EqualTo(new GetServerResponse("1", "test", 5, "ON", "offline", "something.somewhere.io:10000")));

            result = await _serverService.UpdateServer("1", "testuserWithServer");
            Assert.That(result, Is.EqualTo(HttpStatusCode.OK));
            serverInfo = await _serverService.GetServerInfo("1", "testuserWithServer");
            Assert.That(serverInfo, Is.EqualTo(new GetServerResponse("1", "test", 5, "OFF", "offline", "something.somewhere.io:10000")));


        }

        [Test]
        public async Task DeleteServer()
        {
            IDatabaseClient _dbClient = new TestDatabaseClient();
            IServerDeployer _serverDeployer = new TestServerDeployer();
            IServerMonitor _serverMonitor = new TestServerMonitor();
            IServerService _serverService = new ServerService(_dbClient, _serverMonitor, _serverDeployer);

            HttpStatusCode result = await _serverService.DeleteServer("1", "testuserWithServer");
            Assert.That(result, Is.EqualTo(HttpStatusCode.OK));

            GetServerListResponse list = await _serverService.GetServerList("testuserWithServer");
            Assert.That(list.serverIds, Is.EqualTo(new List<string>()));


        }
    }

}
