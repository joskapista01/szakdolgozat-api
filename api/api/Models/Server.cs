namespace api.Models;

public record Server(
    string id, 
    string user,
    string serverName, 
    DateTime createdAt, 
    string serverUrl, 
    int serverPort,
    string serverStatus);