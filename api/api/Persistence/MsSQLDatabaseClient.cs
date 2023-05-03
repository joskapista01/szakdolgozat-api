using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Exceptions;
using Microsoft.Data.SqlClient;

namespace api.Persistence
{
    /// <summary>
    /// MsSQLDatabaseClient is a class that implements the IDatabaseClient interface and handles
    /// communication with an MS SQL Server database.
    /// </summary>
    public class MsSQLDatabaseClient : IDatabaseClient
    {
        private string connectionString;

        /// <summary>
        /// Constructor for MsSQLDatabaseClient that takes a SqlConnectionStringBuilder and initializes 
        /// the connection string and creates the necessary tables if they do not exist.
        /// </summary>
        /// <param name="builder">The SqlConnectionStringBuilder used to create the connection string.</param>
        public MsSQLDatabaseClient(SqlConnectionStringBuilder builder)
        {
            connectionString = builder.ConnectionString;
            SqlConnection databaseConnection = new SqlConnection(connectionString);
            CreateTables();
        }

        /// <summary>
        /// Creates the necessary tables if they do not exist.
        /// </summary>
        private void CreateTables()
        {
            CreateServersTable();
            CreateUsersTable();

        }

        /// <summary>
        /// Creates the servers table if it does not exist.
        /// </summary>
        private void CreateServersTable()
        {
            SqlConnection databaseConnection = new SqlConnection(connectionString);
            databaseConnection.Open();
            string sql = @"IF OBJECT_ID(N'servers', N'U') IS NULL
                CREATE TABLE servers (
                    id varchar(255), 
                    username varchar(255),
                    serverName varchar(255), 
                    createdAt date, 
                    serverUrl varchar(255), 
                    serverPort int,
                    serverStatus varchar(255)
                );";
            
            SqlCommand command = new SqlCommand(sql, databaseConnection);
            command.ExecuteNonQuery();
            databaseConnection.Close();

        }

        /// <summary>
        /// Creates the users table if it does not exist.
        /// </summary>
        private void CreateUsersTable()
        {
            SqlConnection databaseConnection = new SqlConnection(connectionString);
            databaseConnection.Open();
            string sql = @"IF OBJECT_ID(N'users', N'U') IS NULL
                CREATE TABLE users (
                    username varchar(255),
                    password varchar(255)
                );";

            SqlCommand command = new SqlCommand(sql, databaseConnection);
            command.ExecuteNonQuery();
            databaseConnection.Close();

        }

        /// <summary>
        /// Converts a server object to a string for use in SQL queries.
        /// </summary>
        /// <param name="server">The server object to be converted.</param>
        /// <returns>A string representation of the server object.</returns>
        private string ServerToString(Server server)
        {
            return 
                "'" + server.id + "', '"+ 
                server.user + "', '" + 
                server.serverName + "', '" + 
                server.createdAt + "', '" + 
                server.serverUrl + "', " +
                server.serverPort + ", '" +
                server.serverStatus + "'";
        }

        /// <summary>
        /// Converts a SqlDataReader object to a server object.
        /// </summary>
        /// <param name="reader">The SqlDataReader object to be converted.</param>
        /// <returns>A server object.</returns>
        private Server getServerFromSql(SqlDataReader reader)
        {
            return new Server(
                reader.GetString(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetDateTime(3),
                reader.GetString(4),
                reader.GetInt32(5),
                reader.GetString(6)
            );
        }

        /// <summary>
        /// Converts a <see cref="User"/> object to a SQL string for use in database queries.
        /// </summary>
        /// <param name="user">The <see cref="User"/> object to convert.</param>
        /// <returns>A SQL string representing the <see cref="User"/> object.</returns>

        private string UserToString(User user)
        {
            return 
                "'"+user.username + "', '"+ 
                user.password+"'";
        }

        /// <summary>
        /// Converts a <see cref="SqlDataReader"/> object to a <see cref="User"/> object.
        /// </summary>
        /// <param name="reader">The <see cref="SqlDataReader"/> object to convert.</param>
        /// <returns>A <see cref="User"/> object representing the data in the <see cref="SqlDataReader"/> object.</returns>
        private User getUserFromSql(SqlDataReader reader)
        {
            return new User(
                reader.GetString(0),
                reader.GetString(1)
            );
        }

        /// <summary>
        /// Creates a new server in the database.
        /// </summary>
        /// <param name="server">The <see cref="Server"/> object representing the server to create.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean value indicating whether the operation succeeded.</returns
        public async Task<bool> createServer(Server server)
        {
            SqlConnection databaseConnection = new SqlConnection(connectionString);
            try
            {
                databaseConnection.Open();
                string sql = "INSERT INTO servers VALUES (" + ServerToString(server) + ")";
                SqlCommand command = new SqlCommand(sql, databaseConnection);

                await command.ExecuteNonQueryAsync();
                databaseConnection.Close();

                return true;
            }
            catch(Exception e)
            {
                throw e;
            }
            finally
            {
                databaseConnection.Close();
            }

        }

        /// <summary>
        /// Deletes a server with the specified ID and username from the database.
        /// </summary>
        /// <param name="id">The ID of the server to be deleted.</param>
        /// <param name="user">The username associated with the server to be deleted.</param>
        /// <returns>A boolean value indicating whether the operation was successful.</returns>
        public async Task<bool> deleteServer(string id, string user)
        {
            SqlConnection databaseConnection = new SqlConnection(connectionString);
            try
            {
                databaseConnection.Open();
                string sql = "DELETE FROM servers WHERE id = '" + id + "' and username = '" + user + "'";
                SqlCommand command = new SqlCommand(sql, databaseConnection);

                int rows = await command.ExecuteNonQueryAsync();
                if(rows == 0)
                    throw new ServerNotFoundException("Server " + id + " not found");
                databaseConnection.Close();

                return true;
            }
            catch(Exception e)
            {
                throw e;
            }
            finally
            {
                databaseConnection.Close();
            }
        }

        /// <summary>
        /// Retrieves a list of server IDs associated with a given username from the database.
        /// </summary>
        /// <param name="user">The username associated with the servers to be retrieved.</param>
        /// <returns>A list of server IDs.</returns>
        public async Task<List<string>> getServerList(string user)
        {
            SqlConnection databaseConnection = new SqlConnection(connectionString);
            try
            {
                databaseConnection.Open();
                string sql = "SELECT id FROM servers WHERE username = '" + user + "'";
                SqlCommand command = new SqlCommand(sql, databaseConnection);

                SqlDataReader reader = await command.ExecuteReaderAsync();

                List<string> serverIds = new List<string>();

                while(reader.Read())
                {
                    serverIds.Add(reader.GetString(0));
                }
                databaseConnection.Close();

                return serverIds;
            }
            catch(Exception e)
            {
                throw e;
            }
            finally
            {
                databaseConnection.Close();
            }
        }

        /// <summary>
        /// Retrieves server information associated with a given ID and username from the database.
        /// </summary>
        /// <param name="id">The ID of the server to be retrieved.</param>
        /// <param name="user">The username associated with the server to be retrieved.</param>
        /// <returns>A Server object representing the requested server information.</returns>
        public async Task<Server> getServerInfo(string id, string user)
        {
            SqlConnection databaseConnection = new SqlConnection(connectionString);
            try
            {
                databaseConnection.Open();
                string sql = "SELECT * FROM servers WHERE id = '" + id + "' and username = '" + user + "'";
                SqlCommand command = new SqlCommand(sql, databaseConnection);

                SqlDataReader reader = await command.ExecuteReaderAsync();

                if(!reader.HasRows)
                    throw new ServerNotFoundException("Server "+ id +" not found");

                reader.Read();

                Server server = getServerFromSql(reader);
                databaseConnection.Close();

                return server;
            }
            catch(Exception e)
            {
                throw e;
            }
            finally
            {
                databaseConnection.Close();
            }
        }

        /// <summary>
        /// Retrieves user credentials associated with a given username from the database.
        /// </summary>
        /// <param name="username">The username associated with the user credentials to be retrieved.</param>
        /// <returns>A User object representing the requested user credentials.</returns>
        public async Task<User> getUserCreds(string username)
        {
            SqlConnection databaseConnection = new SqlConnection(connectionString);
            try
            {
                databaseConnection.Open();
                string sql = "SELECT * FROM users WHERE username = '" + username + "'";
                SqlCommand command = new SqlCommand(sql, databaseConnection);

                SqlDataReader reader = await command.ExecuteReaderAsync();

                if(!reader.HasRows)
                    throw new UserNotFoundException("Login failed!");

                reader.Read();

                User user = getUserFromSql(reader);
                databaseConnection.Close();

                return user;
            }
            catch(Exception e)
            {
                throw e;
            }
            finally
            {
                databaseConnection.Close();
            }
        }

        /// <summary>
        /// Adds a new user to the database.
        /// </summary>
        /// <param name="user">The user to add to the database.</param>
        /// <returns>A boolean indicating whether the user was successfully added.</returns>
        public async Task<bool> addUser(User user)
        {
            SqlConnection databaseConnection = new SqlConnection(connectionString);
            try
            {
                databaseConnection.Open();
                string sql = "SELECT * FROM users WHERE username = '" + user.username + "'";
                SqlCommand command = new SqlCommand(sql, databaseConnection);
                SqlDataReader reader = await command.ExecuteReaderAsync();

                if(reader.HasRows)
                    throw new UsernameAlreadyTakenException("Username already taken!");

                reader.Close();
            
                sql = "INSERT INTO users VALUES (" + UserToString(user) + ")";
                command = new SqlCommand(sql, databaseConnection);

                var result = await command.ExecuteNonQueryAsync();
                databaseConnection.Close();

                return true;
            }
            catch(Exception e)
            {
                throw e;
            }
            finally
            {
                databaseConnection.Close();
            }
        }

        /// <summary>
        /// Updates the status of a server in the database.
        /// </summary>
        /// <param name="id">The id of the server to update.</param>
        /// <param name="user">The username of the owner of the server.</param>
        /// <param name="status">The new status of the server.</param>
        /// <returns>A boolean indicating whether the server status was successfully updated.</returns>
        public async Task<bool> updateServerStatus(string id, string user, string status)
        {
            SqlConnection databaseConnection = new SqlConnection(connectionString);
            try
            {
                databaseConnection.Open();
                string sql = "UPDATE servers SET serverStatus = '" + status + "' WHERE username = '" + user + "' and id = '" + id + "'";
                SqlCommand command = new SqlCommand(sql, databaseConnection);

                int rows = await command.ExecuteNonQueryAsync();
                if(rows == 0)
                    throw new ServerNotFoundException("Server " + id + " not found");
                databaseConnection.Close();

                return true;
            }
            catch(Exception e)
            {
                throw e;
            }
            finally
            {
                databaseConnection.Close();
            }
        }

        /// <summary>
        /// Gets a list of all active servers in the database.
        /// </summary>
        /// <returns>A list of active servers.</returns>
        public async Task<List<Server>> getActiveServers()
        {
            SqlConnection databaseConnection = new SqlConnection(connectionString);
            try
            {
                databaseConnection.Open();
                string sql = "SELECT * FROM servers WHERE serverStatus = 'ON'";
                SqlCommand command = new SqlCommand(sql, databaseConnection);
                SqlDataReader reader = await command.ExecuteReaderAsync();

                List<Server> activeServers = new List<Server>();

                while(reader.Read())
                {
                    activeServers.Add(getServerFromSql(reader));
                }
                databaseConnection.Close();
                return activeServers;
            }
            catch(Exception e)
            {
                throw e;
            }
            finally
            {
                databaseConnection.Close();
            }
        }

        /// <summary>
        /// Checks if a port is already allocated to a server in the database.
        /// </summary>
        /// <param name="port">The port to check.</param>
        /// <returns>A boolean indicating whether the port is allocated.</returns>
        public async Task<bool> IsPortAllocated(int port)
        {
            SqlConnection databaseConnection = new SqlConnection(connectionString);
            try
            {
                databaseConnection.Open();
                string sql = "SELECT * FROM servers WHERE serverPort = '" + port+"'";
                SqlCommand command = new SqlCommand(sql, databaseConnection);
                SqlDataReader reader = await command.ExecuteReaderAsync();
                bool result = reader.HasRows;
                databaseConnection.Close();

                return result;
            }
            catch(Exception e)
            {
                throw e;
            }
            finally
            {
                databaseConnection.Close();
            }
        }

    }
}