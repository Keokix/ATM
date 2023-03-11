namespace Bankomat.Abstractions
{
    public interface IAdminManager
    {
        bool Authentification(string pin);
        void CreateNewAccount(string username);
        void CreateNewUser(string username, string pin);
        void ToggleFreezeForAccountByIban(int iban);
    }
}