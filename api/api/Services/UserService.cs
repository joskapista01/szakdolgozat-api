namespace api.Services;

using api.Contracts.api;
using api.Persistence;

public class UserService : IUserService
{
    private IDatabaseClient _databaseClient;

    public UserService(IDatabaseClient databaseClient)
    {
        _databaseClient = databaseClient;
    }

    public void RegisterUser(RegisterUserRequest request)
    {
        _databaseClient.addUser(request.username, request.password);
    }
}