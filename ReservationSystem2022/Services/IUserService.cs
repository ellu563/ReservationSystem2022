using ReservationSystem2022.Models;

namespace ReservationSystem2022.Services
{
    public interface IUserService
    {
        // mm. salattua salasanaa varten
        public Task<UserDTO> CreateUserAsync(User user); // palauttaa userDTO:n mutta saa userin

        // tästä alaspain omia
        public Task<UserDTO> GetUserAsync(long id);
        public Task<IEnumerable<UserDTO>> GetUsersAsync();
        public Task<UserDTO> UpdateUserAsync(UserDTO user);
        // ottaa id:n vastaan
        public Task<Boolean> DeleteUserAsync(long id);

    }
}
