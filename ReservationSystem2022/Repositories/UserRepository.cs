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

        // valmis
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

        // valmis: haetaan nyt userNamen perusteella
        public async Task<User> GetUserAsync(string userName)
        {
            User user = _context.Users.Where(x => x.UserName == userName).FirstOrDefault();
            return user; 
        }

        public async Task<User> GetUserIdAsync(long id)
        {
            return await _context.Users.FirstOrDefaultAsync(i => i.Id == id);
        }


        // haetaan kaikki
        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        // paivitetaan
        public async Task<User> UpdateUserAsync(User user) 
        {
            try
            {
                await _context.SaveChangesAsync(); // tallennus
            }
            catch (Exception ex)
            {
                return null; // muutosten tallentaminen ei onnistunut
            }
            return user; // "tallennus on onnistunut"
        }

        // poistetaan
        public async Task<bool> DeleteUserAsync(User user)
        {
            try
            {
                _context.Users.Remove(user); // poistetaan (muistista)
                await _context.SaveChangesAsync(); // saadaan päivitettyä tietokantaan myös tieto
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

    }
}
