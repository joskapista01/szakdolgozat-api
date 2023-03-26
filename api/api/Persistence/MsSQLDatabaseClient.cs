using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Exceptions;
using Microsoft.Data.SqlClient;

namespace api.Persistence;

public class MsSQLDatabaseClient : IDatabaseClient
{
    private SqlConnection databaseConnection;
    public MsSQLDatabaseClient(SqlConnectionStringBuilder builder)
    {
        databaseConnection = new SqlConnection(builder.ConnectionString);
        databaseConnection.Open();
        CreateTables();
    }

    private void CreateTables()
    {
        CreateServersTable();
        CreateUsersTable();

    }

    private void CreateServersTable()
    {
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

    }

    private void CreateUsersTable()
    {
        string sql = @"IF OBJECT_ID(N'users', N'U') IS NULL
            CREATE TABLE users (
                username varchar(255),
                password varchar(255)
            );";

        SqlCommand command = new SqlCommand(sql, databaseConnection);
        command.ExecuteNonQuery();

    }

    private string ServerToString(Server server)
    {
        return 
            server.id + ", "+ 
            server.user + ", " + 
            server.serverName + ", " + 
            server.createdAt + ", " + 
            server.serverUrl + ", " +
            server.serverPort + ", " +
            server.serverStatus;
    }

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

    private string UserToString(User user)
    {
        return 
            user.username + ", "+ 
            user.password;
    }

    private User getUserFromSql(SqlDataReader reader)
    {
        return new User(
            reader.GetString(0),
            reader.GetString(1)
        );
    }
    public async Task<bool> createServer(Server server)
    {
        string sql = "INSERT INTO servers VALUES (" + ServerToString(server) + ")";
        SqlCommand command = new SqlCommand(sql, databaseConnection);

        await command.ExecuteNonQueryAsync();

        return true;
    }

    public async Task<bool> deleteServer(string id, string user)
    {
        string sql = "DELETE FROM servers WHERE id = '" + id + "' and username = '" + user + "'";
        SqlCommand command = new SqlCommand(sql, databaseConnection);

        await command.ExecuteNonQueryAsync();

        return true;
    }

    public async Task<List<string>> getServerList(string user)
    {
        string sql = "SELECT id FROM servers WHERE username = '" + user + "'";
        SqlCommand command = new SqlCommand(sql, databaseConnection);

        SqlDataReader reader = await command.ExecuteReaderAsync();

        List<string> serverIds = new List<string>();

        while(reader.Read())
        {
            serverIds.Add(reader.GetString(0));
        }

        return serverIds;
    }

    public async Task<Server> getServerInfo(string id, string user)
    {
        string sql = "SELECT * FROM servers WHERE id = '" + id + "' username = '" + user + "'";
        SqlCommand command = new SqlCommand(sql, databaseConnection);

        SqlDataReader reader = await command.ExecuteReaderAsync();

        if(!reader.HasRows)
            throw new ServerNotFoundException("Server "+ id +" not found");

        reader.Read();

        Server server = getServerFromSql(reader);

        return server;
    }

    public async Task<User> getUserCreds(string username)
    {
        string sql = "SELECT * FROM users WHERE username = '" + username + "'";
        SqlCommand command = new SqlCommand(sql, databaseConnection);

        SqlDataReader reader = await command.ExecuteReaderAsync();

        if(!reader.HasRows)
            throw new UserNotFoundException("Login failed!");

        reader.Read();

        User user = getUserFromSql(reader);

        return user;
    }

    public async Task<bool> addUser(User user)
    {
        string sql = "SELECT * FROM users WHERE username = '" + user.username + "'";
        SqlCommand command = new SqlCommand(sql, databaseConnection);
        SqlDataReader reader = await command.ExecuteReaderAsync();

        if(reader.HasRows)
            throw new UsernameAlreadyTakenException("Username already taken!");

    
        sql = "INSERT INTO servers VALUES (" + UserToString(user) + ")";
        command = new SqlCommand(sql, databaseConnection);

        await command.ExecuteNonQueryAsync();

        return true;
    }

    public async Task<bool> updateServerStatus(string user, string id, string status)
    {
        string sql = "UPDATE servers SET serverStatus = '" + status + "' WHERE username = '" + user + "' and id = '" + id + "'";
        SqlCommand command = new SqlCommand(sql, databaseConnection);

        int rows = await command.ExecuteNonQueryAsync();
        if(rows == 0)
            throw new ServerNotFoundException("Server " + id + " not found");

        return true;
    }

    public async Task<List<Server>> getActiveServers()
    {
        string sql = "SELECT * FROM servers WHERE serverStatus = ON";
        SqlCommand command = new SqlCommand(sql, databaseConnection);
        SqlDataReader reader = await command.ExecuteReaderAsync();

        List<Server> activeServers = new List<Server>();

        while(reader.Read())
        {
            activeServers.Add(getServerFromSql(reader));
        }
        return activeServers;
    }

    public async Task<bool> IsPortAllocated(int port)
    {
        string sql = "SELECT * FROM servers WHERE serverPort = '" + port+"'";
        SqlCommand command = new SqlCommand(sql, databaseConnection);
        SqlDataReader reader = await command.ExecuteReaderAsync();

        return reader.HasRows;
    }

}
