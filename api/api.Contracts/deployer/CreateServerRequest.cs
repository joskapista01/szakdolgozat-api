namespace api.Contracts.deployer;

public record CreateServerRequest(string serverId, int publicPort);