using System;
using System.Data;
using System.IO;
using System.Linq;
using Dapper;
using DataStorage.Abstractions;
using DataStorage.Abstractions.Exceptions;
using Domain;

namespace DataStorage.SQLite
{
    public class SqLiteUserRepository : IUserRepository
    {
        private IDbConnection _dbConnection;



        public SqLiteUserRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
            Setup();
        }

        public SqLiteUserRepository()
        {
        }

        public void Setup()
        {
            //File.Delete("BankOMatSQLite.db");
            if (!File.Exists("BankOMatSQLite.db"))
            {
                _dbConnection.Open();
                _dbConnection.Execute("CREATE TABLE User(" +
                        "Id    INTEGER NOT NULL," +
                        "Name  TEXT NOT NULL UNIQUE," +
                        "Pin   TEXT NOT NULL," +
                        "PRIMARY KEY(Id AUTOINCREMENT));"
                        );
                _dbConnection.Execute("CREATE TABLE Accounts(" +
                         "Id    INTEGER NOT NULL," +
                         "UserName  TEXT NOT NULL," +
                         "UserId    INTEGER NOT NULL," +
                         "Balance    Numeric NOT NULL," +
                         "CreationDate    TEXT NOT NULL," +
                         "IsFrozen   INTEGER NOT NULL," +
                         "PRIMARY KEY(Id AUTOINCREMENT));"
                         );
                _dbConnection.Close();
            }
        }
        public void Create(User user)
        {
            _dbConnection.Open();
            _dbConnection.Execute("INSERT INTO User (Name, Pin) VALUES (@Name, @Pin)", user);
            _dbConnection.Close();
        }

        public bool DoesUserExist(string username)
        {

            _dbConnection.Open();
            var output = _dbConnection.Query<string>("SELECT Name FROM User WHERE Name = @username", new { username }).Any();
            _dbConnection.Close();
            return output;
        }

        public User[] GetAllUsers()
        {
            var output = _dbConnection.Query<User>("select * from User");
            return output.ToArray();
        }

        public User GetByUsername(string username)
        {
            try
            {
                _dbConnection.Open();
                var output = _dbConnection.QuerySingle<User>("SELECT * FROM User WHERE Name = @username", new { username });
                return output;
            }
            catch (Exception e)
            {
                throw new UserCouldNotBeFoundException($"User  {username} could not be found", e);
            }
            finally
            {
                _dbConnection.Close();
            }
        }
    }
}
