using System.Collections.Generic;

namespace api.Contracts.api;

public record GetServerListResponse(List<string> serverIds);