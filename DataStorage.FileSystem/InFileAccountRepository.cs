using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataStorage.Abstractions;
using DataStorage.Abstractions.Exceptions;
using Domain;
using StaticProxy.Abstractions;

namespace DataStorage.FileSystem
{
    public class InFileAccountRepository : IAccountRepository
    {
        private string _path = new PathProvider().UserPath;
        private IFileOperations _fileOps;
        public List<Account> _accs = new List<Account>();
        private int _accountCount;

        public InFileAccountRepository(IFileOperations fileOps)
        {
            _fileOps = fileOps;
        }

        public void FillAccList()
        {
            string[] directories = _fileOps.GetDirectories(_path);
            foreach(string dir in directories)
            {
                FileInfo userInfo = new FileInfo(dir);
                var UserName = userInfo.Name;
                var AccountPath = _path + "\\" + UserName;

                string[] files = _fileOps.GetFiles(AccountPath);
                Account acc = new Account();

                foreach (string file in files)
                {
                    FileInfo accountInfo = new FileInfo(file);
                    var IBan = accountInfo.Name;
                    var charArray = IBan.ToCharArray();
                    var firstElement = charArray.ElementAt(0);
                    if (Char.IsNumber(firstElement))
                    {
                        string[] content = new string[10];
                        var accPath = _path + "\\" + UserName + "\\" + IBan;
                        content = _fileOps.ReadFromFile(accPath);

                        var balance = content[0];
                        var iban = Convert.ToInt32(content[1]);
                        var userId = Convert.ToInt32(content[2]);
                        var creationDate = content[3];
                        bool isFrozen = Convert.ToBoolean(content[4]);
                        var userName = content[5];

                        acc.UserName = userName;
                        acc.UserId = userId;
                        acc.Id = iban;
                        acc.IsFrozen = isFrozen;
                        acc.CreationDate = Convert.ToDateTime(creationDate);
                        acc.Balance = Convert.ToDecimal(balance);

                        _accs.Add(acc);
                        _accountCount++;
                    }
                }
            }
        }

        public void Create(Account account)
        {
            account.CreationDate = DateTime.UtcNow;
            account.Id = ++_accountCount;
            var accPath = _path + "\\" + account.UserName + "\\" + account.Id + ".txt";
            string[] content = new string[10];

            content[0] = "0";
            content[1] = account.Id.ToString();
            content[2] = account.UserId.ToString();
            content[3] = account.CreationDate.ToString();
            content[4] = "False";
            content[5] = account.UserName;

            _accs.Add(account);

            _fileOps.WriteToFile(accPath, content);

        }

        public Account GetAccountByIban(int iban)
        {
            try
            {
                return _accs.First(acc => acc.Id.Equals(iban));
            }
            catch (InvalidOperationException e)
            {
                throw new AccountNotFoundException($"There is no account for this Iban: {iban}", e);
            }
        }

        public Account[] GetAccountsByUserId(int userId)
        {
            return _accs.Where(acc => acc.UserId.Equals(userId)).ToArray();
        }

        public Account[] GetAccountsByUserName(string username)
        {
            return _accs.Where(acc => acc.UserName.Equals(username)).ToArray();
        }

        public void Save(Account acc)
        {
            string[] content = new string[10];
            
            var accPath = _path + "\\" + acc.UserName + "\\"+ acc.Id + ".txt";
            content = _fileOps.ReadFromFile(accPath);

            content[0] = acc.Balance.ToString();
            content[4] = acc.IsFrozen.ToString();

            _fileOps.WriteToFile(accPath, content);
        }
    }
}
