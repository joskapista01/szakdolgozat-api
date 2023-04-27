using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using api.Contracts.deployer;
using System.Text.Json;
using api.Exceptions;

namespace api.Deploying
{

    public class TestServerDeployer : IServerDeployer {

        
        public TestServerDeployer(){}

        public async Task<HttpStatusCode> CreateServer(string serverId, int serverPort)
        {
            return HttpStatusCode.OK;
        }

        public async Task<HttpStatusCode> UpdateServer(string serverId, int instanceCount)
        {
            return HttpStatusCode.OK;
        }

        public async Task<HttpStatusCode> DeleteServer(string serverId)
        {
            return HttpStatusCode.OK;
        }

    }
}