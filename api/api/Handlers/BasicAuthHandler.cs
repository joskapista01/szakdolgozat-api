using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Net.Http.Headers;
using System.Text;
using api.Persistence;
using api.Models;
using System.Security.Claims;

namespace api.Handlers;

/// <summary>
/// Handler for Basic Authentication.
/// </summary>
public class BasicAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BasicAuthHandler"/> class.
    /// </summary>
    /// <param name="options">The monitor for the authentication scheme options.</param>
    /// <param name="logger">The logger factory.</param>
    /// <param name="encoder">The encoder for URL encoding.</param>
    /// <param name="clock">The system clock.</param>
    /// <param name="databaseClient">The database client.</param>
    public BasicAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> option, 
        ILoggerFactory logger, 
        UrlEncoder encoder,
        ISystemClock clock, IDatabaseClient databaseClient): base(option, logger, encoder, clock) {
            _databaseClient = databaseClient;
        }

        private IDatabaseClient _databaseClient;

        /// <summary>
        /// Authenticates the request using basic authentication.
        /// </summary>
        /// <returns>An asynchronous operation that returns an instance of <see cref="AuthenticateResult"/> when completed.</returns>
        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                if(!Request.Headers.ContainsKey("Authorization"))
                    return AuthenticateResult.Fail("Header not found!");

                var headerValue = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);

                if(headerValue.Parameter is null)
                    return AuthenticateResult.Fail("Unauthorized");


                var bytes = Convert.FromBase64String(headerValue.Parameter);
                string credentials = Encoding.UTF8.GetString(bytes);
                if(!string.IsNullOrEmpty(credentials)){
                    string[] array = credentials.Split(":");
                    string username = array[0];
                    string password = array[1];
                    
                    User userCreds = await _databaseClient.getUserCreds(username);
                    if(userCreds == null || password != userCreds.password)
                        return AuthenticateResult.Fail("Unauthorized");

                    var claim = new []{new Claim(ClaimTypes.Name, username)};
                    var identity = new ClaimsIdentity(claim, Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);


                    return AuthenticateResult.Success(ticket);
                    
                }

                return AuthenticateResult.Fail("Unauthorized");
            } catch(Exception e)
            {
                return AuthenticationExceptionHandler.HandleException(e);
            }
                
        }
}