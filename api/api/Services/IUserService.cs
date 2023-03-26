namespace api.Services;
using api.Contracts.api;

public interface IUserService
{
    public Task<bool> RegisterUser(RegisterUserRequest request);
}