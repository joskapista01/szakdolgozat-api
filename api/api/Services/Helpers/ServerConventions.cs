namespace api.Services.Helpers;

public class ServerConventions
{
    public const int SERVER_CHECK_INTERVAL = 10000; //milisec
    public const int SERVER_MAXIMUM_IDLE_TIME = 60000; //millisec

    public const int START_PORT = 20000;
    public const int END_PORT = 20050;

    public const int DEFAULT_SERVER_PORT = 25565;

    public static string GetServerHostname(string id) 
    {
        return "minecraft-server-"+id+".servers.svc.cluster.local";
    } 


}