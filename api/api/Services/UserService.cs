namespace api.Services;

using api.Contracts.api;
using api.Persistence;
using api.Exceptions;
using System;



public class UserService : IUserService
{
    private IDatabaseClient _databaseClient;

    public UserService(IDatabaseClient databaseClient)
    {
        _databaseClient = databaseClient;
    }

    public void RegisterUser(RegisterUserRequest request)
    {
        ValidateNewCredentials(request)
        try 
        {
            _databaseClient.addUser(request.username, request.password);
        }
        catch(Exception e)
        {
            throw new DatabaseException(e.Message);
        }
        
    }

    private void ValidateNewCredentials(RegisterUserRequest request)
    {
        return ValidateUsername(request.username) && ValidatePassword(request.password);
    }

    private bool ValidateUsername(string username)
    {
        if(username.length >= 4 && IsAlphaNumeric(username))
            return true
        
        throw new RegisterUserException("Invalid username!");
    }

    private bool ValidatePassword(string password)
    {
        if(password.length >= 8 && IsAlphaNumeric(username) && password.Any(char.IsDigit) && password.Any(char.IsLower) && password.Any(char.IsUpper)) 
            return true
        
        throw new RegisterUserException("Invalid password!");
    }

    private bool IsAlphaNumeric(string text)
    {
        foreach (char c in text)
        {
            if(!IsLetterOrDigit(c))
                return false; 
        }
        return true;
    }
}