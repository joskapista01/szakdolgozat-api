using api.Services;
using Microsoft.AspNetCore.Authentication;
using api.Handlers;
using api.Persistence;
using api.Monitoring;
using api.Deploying;

var AllowSpecificOrigins = "AllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddCors();
    builder.Services.AddControllers();
    builder.Host.ConfigureAppConfiguration(config => {
        var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if(envName is not null)
        {
            config.AddJsonFile("appsettings."+envName+".json");
        }
    });
    builder.Services.AddSingleton<IServerService, ServerService>();
    builder.Services.AddSingleton<IUserService, UserService>();
    builder.Services.AddSingleton<IDatabaseClient, TestDatabaseClient>();
    builder.Services.AddSingleton<IServerMonitor>(new RESTServerMonitor(builder.Configuration["MonitorConnection:Hostname"]));
    builder.Services.AddSingleton<IServerDeployer>(new RESTServerDeployer(builder.Configuration["DeployerConnection:Hostname"]));
    builder.Services.AddAuthentication("BasicAuthentication").AddScheme<AuthenticationSchemeOptions, BasicAuthHandler>("BasicAuthentication", null);


    builder.Services.AddCors(options =>
    {
        options.AddPolicy(AllowSpecificOrigins,
            corsBuilder =>
            {
                corsBuilder.WithOrigins(builder.Configuration["FrontendHostname"])
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
    });
   // builder.Services.AddScoped<IUserService, UserService>();
}

var app = builder.Build();
{
    //app.UseHttpsRedirection();

    app.UseCors(AllowSpecificOrigins);

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();
    
    


    app.Run();
}


