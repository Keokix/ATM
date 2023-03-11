using System;
using System.Collections.Generic;
using System.Linq;
using DataStorage.Abstractions;
using DataStorage.Abstractions.Exceptions;
using Domain;

namespace DataStorage.InMemory
{
    public class InMemoryAccountRepository : IAccountRepository
    {
        private int _accountCount;
        private List<Account> _accountStorage = new List<Account>();

        public void Create(Account account)
        {
            account.CreationDate = DateTime.UtcNow;
            account.Id = ++_accountCount;
            _accountStorage.Add(account);
        }

        public Account GetAccountByIban(int iban)
        {
            try
            {
                return _accountStorage.First(acc => acc.Id.Equals(iban));
            }
            catch (InvalidOperationException e)
            {
                throw new AccountNotFoundException($"There is no account for this Iban: {iban}", e);
            }
        }

        public Account[] GetAccountsByUserId(int userId)
        {
                return _accountStorage.Where(acc => acc.UserId.Equals(userId)).ToArray();
        }

        public Account[] GetAccountsByUserName(string username)
        {
                return _accountStorage.Where(acc => acc.UserName.Equals(username)).ToArray();
        }

        public void Save(Account account)
        {
            var oldAcc = GetAccountByIban(account.Id);
            oldAcc = account;
        }
    }
}
