using System;
using System.Text.RegularExpressions;
using Bankomat.Abstractions;
using Bankomat.Abstractions.Exceptions;
using DataStorage.Abstractions;
using DataStorage.Abstractions.Exceptions;
using Domain;

namespace Bankomat
{
    public class AdminManager : IAdminManager
    {
        private readonly IUserRepository _users;
        private readonly IAccountRepository _accounts;
        private readonly Admin _admin = new Admin();

        public AdminManager(IUserRepository userRepository, IAccountRepository accountRepository)
        {
            _users = userRepository;
            _accounts = accountRepository;
        }

        public void CreateNewUser(string username, string pin)
        {
            ValidateUserForNewUserOrThrow(username, pin);
            User user = new User(username, pin);
            _users.Create(user);
        }
        public void CreateNewAccount(string username)
        {
            ValidateUserForNewAccountOrThrow(username);
            User user = _users.GetByUsername(username);

            Account account = new Account(user.Name, user.Id);
            _accounts.Create(account);
        }

        public void ToggleFreezeForAccountByIban(int iban)
        {
            var acc = _accounts.GetAccountByIban(iban);
            acc.IsFrozen = !acc.IsFrozen;
            _accounts.Save(acc);
        }
        
        private void ValidateUserForNewAccountOrThrow(string username)
        {
            if (!_users.DoesUserExist(username))
                throw new UserCouldNotBeFoundException($"User with username: '{username}' could not be found");
        }
        private void ValidateUserForNewUserOrThrow(string username, string pin)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException();

            if (!Regex.IsMatch(username, "^[a-zA-Z]{2,8}$"))
                throw new UsernameIsNotValidException($"Username '{username}' ist not valid, please use 2-8 letters with only a-Z");
            
            if (_users.DoesUserExist(username))
                throw new UsernameAlreadyExistException($"User with username: '{username}' already exists");

            if (string.IsNullOrWhiteSpace(pin))
                throw new PinNotValidException();

            if (!Regex.IsMatch(pin, "^[0-9]{4}$"))
                throw new PinNotValidException($"Pin '{pin} is not valid, please use only 4 digits 0-9'");
        }

        public bool Authentification(string pin)
        {
            if (pin != _admin.PIN)
            {
                throw new PinNotValidException($"Pin is invalid");
            }
            return _admin.PIN == pin;
        }
    }
}
