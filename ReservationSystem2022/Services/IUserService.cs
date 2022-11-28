using ReservationSystem2022.Models;

namespace ReservationSystem2022.Services
{
    public interface IUserService
    {
        // mm. salattua salasanaa varten
        public Task<UserDTO> CreateUserAsync(User user); // palauttaa userDTO:n mutta saa userin
    }
}
