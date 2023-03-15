namespace api.Contracts.monitor;

public record GetServerInfoRequest(string serverHostname, int serverPort);