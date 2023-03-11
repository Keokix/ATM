using Bankomat.Abstractions;
using Bankomat.Abstractions.Exceptions;
using DataStorage.Abstractions;
using Domain;

namespace Bankomat
{
    public class UserManager : IUserManager
    {
        private readonly IUserRepository _userRepo;
        private readonly IAccountRepository _accRepo;

        public UserManager(IUserRepository userRepo, IAccountRepository accRepo)
        {
            _userRepo = userRepo;
            _accRepo = accRepo;
        }
        public bool Authentification(string userName, string pin)
        {
            var user = _userRepo.GetByUsername(userName);
            if (pin != user.Pin)
            {
                throw new PinNotValidException($"Pin for user '{userName}' is invalid");
            }
            return pin == user.Pin;
        }

        public void Deposit(int iban, decimal amount)
        {
            Account account = _accRepo.GetAccountByIban(iban);
            if(amount < 0)
            {
                throw new AmountToDepositCantBeNegativeException($"Amount cant be negative");
            }
            account.Balance += amount;
            _accRepo.Save(account);
        }

        public void Withdraw(int iban, decimal amount)
        {
            var account = _accRepo.GetAccountByIban(iban);
            if (amount < 0)
            {
                throw new AmountToDepositCantBeNegativeException($"Amount cant be negative");
            }
            account.Balance -= amount;
            _accRepo.Save(account);
        }

        public void Pay(int senderIban, int receiverIban, decimal amount)
        {
            var receiver = _accRepo.GetAccountByIban(receiverIban);
            var sender = _accRepo.GetAccountByIban(senderIban);
            if (amount < 0)
            {
                throw new AmountToDepositCantBeNegativeException($"Amount cant be negative");
            }
            receiver.Balance += amount;
            sender.Balance -= amount;
            _accRepo.Save(sender);
            _accRepo.Save(receiver);
        }

    }
}
