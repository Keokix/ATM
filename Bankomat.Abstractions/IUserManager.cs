namespace Bankomat.Abstractions
{
    public interface IUserManager
    {
        bool Authentification(string userName, string pin);
        void Deposit(int iban, decimal amount);
        void Pay(int senderIban, int receiverIban, decimal amount);
        void Withdraw(int iban, decimal amount);
    }
}