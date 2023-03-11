using Domain;

namespace DataStorage.Abstractions
{
    public interface IAccountRepository
    {
        void Create(Account account);
        Account GetAccountByIban(int iban);
        Account[] GetAccountsByUserName(string username);
        Account[] GetAccountsByUserId(int userId);
        void Save(Account account);
    }
}
