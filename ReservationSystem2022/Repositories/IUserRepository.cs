using ReservationSystem2022.Models;

namespace ReservationSystem2022.Repositories
{
    public interface IUserRepository
    {
        // Itemin mukana tulee käyttäjänimi
        public Task<User> GetUserAsync(String userName);

        public Task<User> AddUserAsync(User user);

        // omat
        public Task<User> GetUserIdAsync(long id); // haetaan id:n perusteella
        public Task<IEnumerable<User>> GetUsersAsync(); 
        public Task<Boolean> DeleteUserAsync(User user); // user user = add, update, delete
        public Task<User> UpdateUserAsync(User user); 
    }
}
