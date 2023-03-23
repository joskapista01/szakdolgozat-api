namespace api.Services;

using api.Contracts.api;
using api.Persistence;
using api.Exceptions;
using System;

using api.Services.Helpers;



public class UserService : IUserService
{
    private IDatabaseClient _databaseClient;

    public UserService(IDatabaseClient databaseClient)
    {
        _databaseClient = databaseClient;
    }

    public void RegisterUser(RegisterUserRequest request)
    {
        UserRegistration.ValidateNewCredentials(request.username, request.password);
        try 
        {
            _databaseClient.addUser(request.username, request.password);
        }
        catch(Exception e)
        {
            throw new DatabaseException(e.Message);
        }
        
    }



}