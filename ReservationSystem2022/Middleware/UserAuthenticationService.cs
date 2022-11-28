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

        Task<bool> IsAllowed(String username, User user); // eli parametrin mukaan menee oikein (kun kaikilla sama nimi)

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

        // tekee käyttäjätunnus ja salasanatarkastuksen
        public async Task<User> Authenticate(string username, string password)
        {
            // etitaan contextista, halutaan että UserName on sama kun username
            User? user = await _context.Users.Where(x => x.UserName == username).FirstOrDefaultAsync();

            if(user == null) // eli nyt etitaan suoraan tietokannasta 
            {
                return null;
            }
            // onko oikea salasana, eli lasketaan uudelleen tiiviste arvo
            byte[] salt = user.Salt; // millä suolan arvolla käyttäjälle on alunperin laskettu tiiviste arvo

            string hashedPassWord = Convert.ToBase64String(KeyDerivation.Pbkdf2( // pdKDF2 laskentafunktio
                password: password, // eli otetaan se salasana minkä käyttäjä on syöttänyt (parametri)  
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000, 
                numBytesRequested: 256 / 8));

            if(hashedPassWord != user.Password) // jos tietokannasta saatu ja käyttäjän antama on eri
            {
                return null;
            }
            // käyttäjä on tunnistettu
            return user;
        }

        // pääsynhallinta
        // onko talla kayttajanimella oikeus kasitella tata itemia, eli onko itemin omistaja
        public async Task<bool> IsAllowed(string username, ItemDTO item)
        {
            // onko käyttäjä olemassa, mennään tietokantaan, firstordefaultasync = palauttaa joko yhden kayttajan tai ei mitaan
            // onko username sama kun username mikä saatiin parametrinä
            User? user = await _context.Users.Where(x => x.UserName == username).FirstOrDefaultAsync();
            // haetaan item tietokannasta, haetaan tuon itemin id:n perusteella, halutaan myös owneri mukaan
            Item? dbItem = await _context.Items.Include(i => i.Owner).FirstOrDefaultAsync(i => i.Id == item.Id);

            // löytyikö kayttaja
            if(user == null)
            {
                return false; // ei oikeutta
            }

            if(dbItem == null && item.Owner == user.UserName)
            {
                return true;
            }

            if(dbItem == null)
            {
                return false;
            }

            // onko parametri user käyttäjä joka haluaa tehä tuohon parametri itemiin muutoksia, onko se sama kun
            // tietokannassa olevan itemin omistaja
            if(user.Id == dbItem.Owner.Id)
            {
                return true;
            }
            return false;
        }

        // kayttaja haluaa editoida käyttäjän tietoja, eli pääsee muokkaamaan vaan omia tietoja
        // userin kanssa hieman erilainen toteutus
        public async Task<bool> IsAllowed(string username, User user) 
        {
            User? dbUser = await _context.Users.Where(x => x.UserName == user.UserName).FirstOrDefaultAsync();

            if (dbUser != null && dbUser.UserName == username)
            {
                return true;
            }
            return false;
        }

        // onko oikeus muokata varausta
        public async Task<bool> IsAllowed(string username, ReservationDTO reservation)
        {
            User? user = await _context.Users.Where(x => x.UserName == username).FirstOrDefaultAsync();
            // katotaan onko tietokannassa tälle merkattu tuo varaus
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
