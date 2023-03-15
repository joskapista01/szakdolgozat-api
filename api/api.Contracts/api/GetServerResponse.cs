namespace api.Contracts.api;

public record GetServerResponse(
    string id,
    string serverName,
    int playerCount,
    string serverStatus,
    string serverState,
    string serverUrl);