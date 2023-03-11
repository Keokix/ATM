using System;
using System.Data;
using System.Linq;
using Dapper;
using DataStorage.Abstractions;
using DataStorage.Abstractions.Exceptions;
using Domain;

namespace DataStorage.SQLite
{
    public class SqLiteAccountRepository : IAccountRepository
    {
        private IDbConnection _dbConnection;

        public SqLiteAccountRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public void Create(Account account)
        {
            account.CreationDate = DateTime.UtcNow;
            account.IsFrozen = false;
            _dbConnection.Open();
            _dbConnection.Execute("INSERT INTO Accounts(UserName, UserId, Balance, CreationDate, IsFrozen) " +
                "VALUES(@UserName, @UserId, @Balance, @CreationDate, @IsFrozen)", account);
            _dbConnection.Close();
        }

        public Account GetAccountByIban(int iban)
        {
            try
            {
                _dbConnection.Open();
                var output = _dbConnection.QuerySingle<Account>("SELECT * FROM Accounts WHERE Id = @iban", new { iban });
                return output;
            }
            catch (UserCouldNotBeFoundException e)
            {
                Console.WriteLine($"No account with this iban found: {iban}");
                return null;
            }
            finally
            {
                _dbConnection.Close();
            }
        }

        public Account[] GetAccountsByUserId(int userId)
        {
            try
            {
                _dbConnection.Open();
                var output = _dbConnection.Query<Account>("SELECT * FROM Accounts WHERE UserId = @userId", new { userId }).ToArray();
                return output;
            }
            catch (Exception e)
            {
                throw new UserCouldNotBeFoundException($"User with Id: {userId} could not be found", e);
            }
            finally
            {
                _dbConnection.Close();
            }
        }

        public Account[] GetAccountsByUserName(string username)
        {
            try
            {
                _dbConnection.Open();
                var output = _dbConnection.Query<Account>("SELECT * FROM Accounts WHERE UserName = @username", 
                    new { username }).ToArray();
                return output;
            }
            catch (Exception e)
            {
                throw new UserCouldNotBeFoundException($"User {username} could not be found", e);
            }
            finally
            {
                _dbConnection.Close();
            }
        }

        public void Save(Account account)
        {
            _dbConnection.Open();
            _dbConnection.Execute("UPDATE Accounts SET Balance = @Balance, IsFrozen = @IsFrozen WHERE Id = @Id", account);
            _dbConnection.Close();
        }
    }
}
