namespace api.Services;

using api.Contracts.api;
using api.Persistence;
using api.Exceptions;
using System;
using api.Models;

using api.Services.Helpers;



public class UserService : IUserService
{
    private IDatabaseClient _databaseClient;

    public UserService(IDatabaseClient databaseClient)
    {
        _databaseClient = databaseClient;
    }

    public async Task<bool> RegisterUser(RegisterUserRequest request)
    {
        UserRegistration.ValidateNewCredentials(request.username, request.password);
        
        User user = new User(request.username, request.password);

        return await _databaseClient.addUser(user);
    }
}