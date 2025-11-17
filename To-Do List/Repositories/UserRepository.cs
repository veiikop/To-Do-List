using To_Do_List.Models;

namespace To_Do_List.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly APIDBContect _context;

        public UserRepository(APIDBContect context)
        {
            _context = context;
        }

        public User? GetById(int id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id);
        }

        public User? GetByEmailOrUsername(string emailOrUsername)
        {
            return _context.Users
                .FirstOrDefault(u => u.Email == emailOrUsername || u.Username == emailOrUsername);
        }

        public User Create(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }

        public bool ExistsByEmail(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }

        public bool ExistsByUsername(string username)
        {
            return _context.Users.Any(u => u.Username == username);
        }
    }
}
