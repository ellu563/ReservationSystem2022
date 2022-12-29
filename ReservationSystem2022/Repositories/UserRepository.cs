using Microsoft.EntityFrameworkCore;
using ReservationSystem2022.Models;

namespace ReservationSystem2022.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ReservationContext _context;

        public UserRepository(ReservationContext context)
        {
            _context = context;
        }

        // tää oli kanssa taalla
        public async Task<User> AddUserAsync(User user)
        {
            _context.Users.Add(user);
            try
            {
                await _context.SaveChangesAsync(); // tallennetaan
            }
            catch(Exception e)
            {
                return null;
            }
            return user;
        }

        public Task<bool> DeleteUserAsync(User user)
        {
            throw new NotImplementedException();
        }

        // tää oli taalla
        public async Task<User> GetUserAsync(string userName)
        {
            User user = _context.Users.Where(x => x.UserName == userName).FirstOrDefault();
            return user; 
        }

        // tää on ite tehty
        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }
    }
}
