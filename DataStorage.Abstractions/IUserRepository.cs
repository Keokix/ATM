using Domain;

namespace DataStorage.Abstractions
{
    public interface IUserRepository
    {
        bool DoesUserExist(string username);
        User GetByUsername(string username);
        void Create(User user);
        User[] GetAllUsers();

    }
}
