using To_Do_List.Models;

namespace To_Do_List.Repositories
{
    public interface IUserRepository
    {
        User? GetById(int id);
        User? GetByEmailOrUsername(string emailOrUsername);
        User Create(User user);
        User Update(User user);
        IEnumerable<User> GetAll();
        bool ExistsByEmail(string email);
        bool ExistsByUsername(string username);
    }
}
