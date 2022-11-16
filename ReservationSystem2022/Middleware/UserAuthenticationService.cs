using Microsoft.AspNetCore.Cryptography.KeyDerivation;
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
    }
}
