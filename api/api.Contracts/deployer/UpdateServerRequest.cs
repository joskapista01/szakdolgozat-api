namespace api.Contracts.deployer;

public record UpdateServerRequest(string serverId, int replicaCount);