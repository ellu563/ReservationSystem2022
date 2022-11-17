using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using ReservationSystem2022.Models;

namespace ReservationSystem2022.Middleware
{
    // etsii tietokannasta loytyyko kayttaja
    public interface IUserAuthenticationService
    {
        Task<User> Authenticate(string username, string password);
        Task<bool> IsAllowed(String username, ItemDTO item); // käyttäjä haluaa tehdä kohteelle jotain, saako se

        Task<bool> IsAllowed(String username, User user);

        Task<bool> IsAllowed(String username, ReservationDTO reservation);
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
            // onko oikea salasana
            byte[] salt = user.Salt; 

            string hashedPassWord = Convert.ToBase64String(KeyDerivation.Pbkdf2( // pdKDF2 laskentafunktio
                password: user.Password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000, // kuinka pitkaan algoritmia pyoritetaan
                numBytesRequested: 256 / 8));

            if(hashedPassWord != user.Password)
            {
                return null;
            }


            return user;
        }

        public async Task<bool> IsAllowed(string username, ItemDTO item)
        {
            // onko käyttäjä olemassa, mennään tietokantaan, palauttaa joko yhden kayttajan tai ei mitaan
            User user = await _context.Users.Where(x => x.UserName == username).FirstOrDefaultAsync();

            // löytyikö kayttaja
            if(user == null)
            {
                return false;
            }
            // onko kayttajanimi sama kun itemissä on merkattu owner
            if(user.UserName == item.Owner)
            {
                return true;
            }
            return false;
        }

        public Task<bool> IsAllowed(string username, User user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsAllowed(string username, ReservationDTO reservation)
        {
            throw new NotImplementedException();
        }
    }
}
