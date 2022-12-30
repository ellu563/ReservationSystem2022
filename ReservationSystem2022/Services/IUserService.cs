using ReservationSystem2022.Models;

namespace ReservationSystem2022.Services
{
    public interface IUserService
    {
        // mm. salattua salasanaa varten
        public Task<UserDTO> CreateUserAsync(User user); // palauttaa userDTO:n mutta saa userin

        // tästä alaspain omia
        public Task<UserDTO> GetUserIdAsync(long id); // id:n perusteella
        public Task<UserDTO> GetUserAsync(String userName); 
        public Task<IEnumerable<UserDTO>> GetUsersAsync(); 
        public Task<UserDTO> UpdateUserAsync(User user); 
        
        public Task<Boolean> DeleteUserAsync(long id); 

    }
}
