using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Net.Http.Headers;
using System.Text;
using api.Persistence;
using api.Models;
using System.Security.Claims;

namespace api.Handlers;

public class BasicAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public BasicAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> option, 
        ILoggerFactory logger, 
        UrlEncoder encoder,
        ISystemClock clock, IDatabaseClient databaseClient): base(option, logger, encoder, clock) {
            _databaseClient = databaseClient;
        }

        private IDatabaseClient _databaseClient;

        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if(!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Header not found!");

            var headerValue = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            var bytes = Convert.FromBase64String(headerValue.Parameter);
            string credentials = Encoding.UTF8.GetString(bytes);
            if(!string.IsNullOrEmpty(credentials)){
                string[] array = credentials.Split(":");
                string username = array[0];
                string password = array[1];
                
                User userCreds = _databaseClient.getUserCreds(username);
                if(userCreds == null || password != userCreds.password)
                    return AuthenticateResult.Fail("Unauthorized");

                var claim = new []{new Claim(ClaimTypes.Name, username)};
                var identity = new ClaimsIdentity(claim, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);


                return AuthenticateResult.Success(ticket);
                
            }

            return AuthenticateResult.Fail("Unauthorizedddd");
            
        }
}