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

        Task<bool> IsAllowed(String username, User user); // eli parametrin mukaan menee oikein

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

        // tekee salasanatarkastuksen
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
                password: password,
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

        // pääsynhallinta
        // onko talla kayttajanimella oikeus kasitella tata itemia
        public async Task<bool> IsAllowed(string username, ItemDTO item)
        {
            // onko käyttäjä olemassa, mennään tietokantaan, palauttaa joko yhden kayttajan tai ei mitaan
            User? user = await _context.Users.Where(x => x.UserName == username).FirstOrDefaultAsync();
            // haetaan item tietokannasta, haetaan tuon itemin id:n perusteella
            Item? dbItem = await _context.Items.Include(i => i.Owner).FirstOrDefaultAsync(i => i.Id == item.Id);

            // löytyikö kayttaja
            if(user == null || dbItem == null)
            {
                return false;
            }
            // onko sama
            if(user.Id == dbItem.Owner.Id)
            {
                return true;
            }
            return false;
        }

        // kayttaja haluaa editoida käyttäjän tietoja
        public async Task<bool> IsAllowed(string username, User user) // mitä vaan mitä kutsun tekijä lähettänyt
        {
            User? dbUser = await _context.Users.Where(x => x.UserName == user.UserName).FirstOrDefaultAsync();

            if (dbUser != null && dbUser.UserName == username)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> IsAllowed(string username, ReservationDTO reservation)
        {
            User? user = await _context.Users.Where(x => x.UserName == username).FirstOrDefaultAsync();
            Reservation? dbReservation = await _context.Reservations.Include(i => i.Owner).FirstOrDefaultAsync(i => i.Id == reservation.Id);
            
            if(user == null || dbReservation == null)
            {
                return false;
            }
            // tarkistetaan myös omistaja id
            if(user.Id == dbReservation.Owner.Id)
            {
                return true;
            }
            return false;
        }
    }
}
