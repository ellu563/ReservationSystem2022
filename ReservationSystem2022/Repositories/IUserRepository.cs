using ReservationSystem2022.Models;

namespace ReservationSystem2022.Repositories
{
    public interface IUserRepository
    {
        // Itemin mukana tulee käyttäjänimi
        public Task<User> GetUserAsync(String userName);

        public Task<User> AddUserAsync(User user);
    }
}
