using api.Services;
using Microsoft.AspNetCore.Authentication;
using api.Handlers;
using api.Persistence;
using api.Monitoring;
using api.Deploying;
using System.IO;

var AllowAllOrigins = "AllowAllOrigins";
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(80);
});
{
    builder.Services.AddCors();
    builder.Services.AddControllers();
    builder.Host.ConfigureAppConfiguration((hostingContext, config) => {

        config.Sources.Clear();

        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        config.AddJsonFile("appsettings."+env+".json",
                            optional: false, reloadOnChange: true);
    });
    builder.Services.AddSingleton<IServerService, ServerService>();
    builder.Services.AddSingleton<IUserService, UserService>();
    builder.Services.AddSingleton<IDatabaseClient, TestDatabaseClient>();
    builder.Services.AddSingleton<IServerMonitor>(new RESTServerMonitor(builder.Configuration["MonitorConnection:Hostname"]));
    builder.Services.AddSingleton<IServerDeployer>(new RESTServerDeployer(builder.Configuration["DeployerConnection:Hostname"]));
    builder.Services.AddAuthentication("BasicAuthentication").AddScheme<AuthenticationSchemeOptions, BasicAuthHandler>("BasicAuthentication", null);


    builder.Services.AddCors(options =>
    {
        options.AddPolicy(AllowAllOrigins,
            policy => {
                policy          
                 .AllowAnyOrigin() 
                 .AllowAnyMethod()
                 .AllowAnyHeader();
            });
    });
}



var app = builder.Build();
{

    app.UseCors(AllowAllOrigins);

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();
    
    app.Run();
}

