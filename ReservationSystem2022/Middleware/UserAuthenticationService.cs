using Microsoft.EntityFrameworkCore;
using ReservationSystem2022.Models;

namespace ReservationSystem2022.Middleware
{
    // etsii tietokannasta loytyyko kayttaja
    public interface IUserAuthenticationService
    {
        Task<User> Authenticate(string username, string password);
    }
    public class UserAuthenticationService : IUserAuthenticationService // toteuttaa interfacen
    {
        // tarvitaan yhteystietokantaan/tietokanta konteksti
        private readonly ReservationContext _context;

        public UserAuthenticationService(ReservationContext context)
        {
            _context = context;
        }

        public async Task<User> Authenticate(string username, string password)
        {
            // etitaan contextista, halutaan että UserName on sama kun username
            User? user = await _context.Users.Where(x => x.UserName == username).FirstOrDefaultAsync();

            if(user == null) // eli nyt etitaan suoraan tietokannasta 
            {
                return null;
            }
            if(user.Password != password)
            {
                return null;
            }
            // on tarkistettu kayttajanimi ja salasana joten palautetaan
            return user;
        }
    }
}
