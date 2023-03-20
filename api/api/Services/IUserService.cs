namespace api.Services;
using api.Contracts.api;

public interface IUserService
{
    public void RegisterUser(RegisterUserRequest request);
}