using api.Contracts.api;
using api.Persistence;
using api.Exceptions;
using System;
using api.Models;

using api.Services.Helpers;


namespace api.Services
{
    /// <summary>
    /// Service responsible for managing user accounts.
    /// </summary>
    public class UserService : IUserService
    {
        private IDatabaseClient _databaseClient;

        /// <summary>
        /// Constructor for the UserService.
        /// </summary>
        public UserService(IDatabaseClient databaseClient)
        {
            _databaseClient = databaseClient;
        }

        /// <summary>
        /// Registers a new user with the given credentials.
        /// </summary>
        /// <param name="request">The RegisterUserRequest object containing the new user's credentials.</param>
        /// <returns>A boolean value indicating whether the registration was successful.</returns>
        public async Task<bool> RegisterUser(RegisterUserRequest request)
        {
            UserRegistration.ValidateNewCredentials(request.username, request.password);
            
            User user = new User(request.username, request.password);

            return await _databaseClient.addUser(user);
        }
    }
}