using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Services;
using api.Persistence;
using api.Monitoring;
using api.Deploying;
using api.Contracts;
using api.Contracts.api;
using NUnit.Framework;
using api.Exceptions;

namespace api.Tests.UserTests
{
    [TestFixture]
    internal class UserServiceTests
    {
        [Test]
        public async Task RegisterUserWithValidCredentials()
        {
            IDatabaseClient _dbClient = new TestDatabaseClient();
            IUserService _userService = new UserService(_dbClient);

            RegisterUserRequest request = new RegisterUserRequest("test", "Test1234");

            bool result = await _userService.RegisterUser(request);

            Assert.AreEqual(true, result);

        }

        [Test]
        public async Task RegisterUserWithUsernameThatAlreadyExists()
        {
            IDatabaseClient _dbClient = new TestDatabaseClient();
            IUserService _userService = new UserService(_dbClient);

            RegisterUserRequest request = new RegisterUserRequest("test", "Test1234");

            bool result = await _userService.RegisterUser(request);
            try
            {
                result = await _userService.RegisterUser(request);
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.GetType(), typeof(UsernameAlreadyTakenException));
            }
        }

        [Test]
        public async Task RegisterUserWithTooShortUsername()
        {
            IDatabaseClient _dbClient = new TestDatabaseClient();
            IUserService _userService = new UserService(_dbClient);

            RegisterUserRequest request = new RegisterUserRequest("tes", "Test1234");

            try
            {
                bool result = await _userService.RegisterUser(request);
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.GetType(), typeof(InvalidUsernameException));
            }
        }

        [Test]
        public async Task RegisterUserWithTooLongUsername()
        {
            IDatabaseClient _dbClient = new TestDatabaseClient();
            IUserService _userService = new UserService(_dbClient);

            RegisterUserRequest request = new RegisterUserRequest("tescdsfffffffffffffffffffffffffffffffffffffffffffff", "Test1234");

            try
            {
                bool result = await _userService.RegisterUser(request);
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.GetType(), typeof(InvalidUsernameException));
            }
        }

        [Test]
        public async Task RegisterUserWithUsernameContainingInvalidCharcter()
        {
            IDatabaseClient _dbClient = new TestDatabaseClient();
            IUserService _userService = new UserService(_dbClient);

            RegisterUserRequest request = new RegisterUserRequest("tes__", "Test1234");

            try
            {
                bool result = await _userService.RegisterUser(request);
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.GetType(), typeof(InvalidUsernameException));
            }
        }

        [Test]
        public async Task RegisterUserWithTooLongPassword()
        {
            IDatabaseClient _dbClient = new TestDatabaseClient();
            IUserService _userService = new UserService(_dbClient);

            RegisterUserRequest request = new RegisterUserRequest("test", "Test1234dasdasdasdasdasdasdasddasds");

            try
            {
                bool result = await _userService.RegisterUser(request);
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.GetType(), typeof(InvalidPasswordException));
            }
        }
        [Test]
        public async Task RegisterUserWithPasswordNotContainingNumber()
        {
            IDatabaseClient _dbClient = new TestDatabaseClient();
            IUserService _userService = new UserService(_dbClient);

            RegisterUserRequest request = new RegisterUserRequest("test", "Testtttt");

            try
            {
                bool result = await _userService.RegisterUser(request);
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.GetType(), typeof(InvalidPasswordException));
            }
        }

        [Test]
        public async Task RegisterUserWithTooShortPassword()
        {
            IDatabaseClient _dbClient = new TestDatabaseClient();
            IUserService _userService = new UserService(_dbClient);

            RegisterUserRequest request = new RegisterUserRequest("test", "Test123");

            try
            {
                bool result = await _userService.RegisterUser(request);
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.GetType(), typeof(InvalidPasswordException));
            }
        }

        [Test]
        public async Task RegisterUserWithPasswordNotContainingSmallLetter()
        {
            IDatabaseClient _dbClient = new TestDatabaseClient();
            IUserService _userService = new UserService(_dbClient);

            RegisterUserRequest request = new RegisterUserRequest("test", "TEST1234");

            try
            {
                bool result = await _userService.RegisterUser(request);
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.GetType(), typeof(InvalidPasswordException));
            }
        }

        [Test]
        public async Task RegisterUserWithPasswordNotContainingCapitalLetter()
        {
            IDatabaseClient _dbClient = new TestDatabaseClient();
            IUserService _userService = new UserService(_dbClient);

            RegisterUserRequest request = new RegisterUserRequest("test", "test1234");

            try
            {
                bool result = await _userService.RegisterUser(request);
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.GetType(), typeof(InvalidPasswordException));
            }
        }

        [Test]
        public async Task RegisterUserWithPasswordContainingForbiddenCharacter()
        {
            IDatabaseClient _dbClient = new TestDatabaseClient();
            IUserService _userService = new UserService(_dbClient);

            RegisterUserRequest request = new RegisterUserRequest("test", "Tes_1234");

            try
            {
                bool result = await _userService.RegisterUser(request);
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.GetType(), typeof(InvalidPasswordException));
            }
        }
    }
}
