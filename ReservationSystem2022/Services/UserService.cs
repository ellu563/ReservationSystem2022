using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using ReservationSystem2022.Models;
using ReservationSystem2022.Repositories;
using System.Security.Cryptography;

namespace ReservationSystem2022.Services
{
    public class UserService : IUserService
    {
        // pitää saada myös tallennettua
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)   
        { 
            _repository = repository;
        }

        // luodaan tallennettavan käyttäjän tiedot
        // luodaan salattu salasana/salasanatiiviste, jotka tallennetaan sitten tietokantaan (repository tasolla)
        public async Task<UserDTO> CreateUserAsync(User user)
        {
            // suola
            byte[] salt = new byte[128 / 8];
            // pitää olla jokaiselle käyttäjälle erilainen = satunnaisgeneraattori
            using(var rng=RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt); // saadaan satunnaisarvo taulukkoon
            } 
            // talleen saadaan hashed password, joka tallennetaan tietokantaan
            string hashedPassWord = Convert.ToBase64String(KeyDerivation.Pbkdf2( // pdKDF2 laskentafunktio
                password: user.Password, // eli otetaan käyttäjän salasana
                salt: salt, // suola mukana salasanassa
                prf: KeyDerivationPrf.HMACSHA256, // käytetään sha256
                iterationCount: 10000, // kuinka pitkaan algoritmia pyoritetaan, menee enemmän aikaa (parempi turvallisuudelle)
                numBytesRequested: 256/8)); // 256 bittinen arvo

            User newUser = new User // luodaan uusi käyttäjä olio ja sille noi kentät
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Salt = salt,
                Password = hashedPassWord,
                JoinDate = DateTime.Now
            };
          
            newUser = await _repository.AddUserAsync(newUser); // heitetään repositorylle ja tallennetaan kantaan

            if(newUser == null){ // tarkistetaan
                return null;
            }
            return UserToDTO(newUser);
        }

        // hae usernamen perusteella
        public async Task<UserDTO> GetUserAsync(string userName) // tanne vaihdettu myos
        {
            User user = await _repository.GetUserAsync(userName); // ..service kutsuu repositorya

            if (user != null) // tarkistetaan löytyykö sieltä mitään
            {
                return UserToDTO(user); // jos löytyy niin tehdään siitä DTO
            }
            return null; // jos ei ole löydetty, palautetaan null
        }

        // haetaan id:n perusteella
        public async Task<UserDTO> GetUserIdAsync(long id)
        {
            User user = await _repository.GetUserIdAsync(id); // ..service kutsuu repositorya

            if (user != null) // tarkistetaan löytyykö sieltä mitään
            {
                return UserToDTO(user); // jos löytyy niin tehdään siitä DTO
            }
            return null; // jos ei ole löydetty, palautetaan null
        }

        // hae kaikki userit
        public async Task<IEnumerable<UserDTO>> GetUsersAsync()
        {
            IEnumerable<User> users = await _repository.GetUsersAsync();
            List<UserDTO> result = new List<UserDTO>();
            foreach (User i in users)
            {
                result.Add(UserToDTO(i));
            }
            return result; // palautetaan
        }

        // paivitetaan useria
        public async Task<UserDTO> UpdateUserAsync(User user)
        {
            User oldUser = await _repository.GetUserAsync(user.UserName); // user.UserName?

            if (oldUser == null) 
            {
                return null;
            }
           
            oldUser.UserName = user.UserName;
            oldUser.FirstName = user.FirstName;
            oldUser.LastName = user.LastName;
            oldUser.JoinDate = user.JoinDate;
            oldUser.LoginDate = user.LoginDate;

            // nyt kun kaikkiin kenttiin tallennettu uusi arvo niin updatetaan se tieto
            User updatedRes = await _repository.UpdateUserAsync(oldUser);
            if (updatedRes == null) // joku on mennyt vikaan
            {
                return null;
            }
            return UserToDTO(updatedRes);
        }

        // poistetaan id:n perusteella
        public async Task<bool> DeleteUserAsync(long id)
        {
            User oldUser = await _repository.GetUserIdAsync(id);
            if (oldUser == null)
            {
                return false;
            }
            return await _repository.DeleteUserAsync(oldUser);
        }

        // tätä ei käytetä missään
        private User DTOToUser(UserDTO user)
        {
            User newUser = new User(); // luodaan uusi käyttäjä olio ja sille noi kentät
            /*
            // suola
            byte[] salt = new byte[128 / 8];
            // pitää olla jokaiselle käyttäjälle erilainen = satunnaisgeneraattori
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt); // saadaan satunnaisarvo taulukkoon
            }
            // talleen saadaan hashed password, joka tallennetaan tietokantaan
            string hashedPassWord = Convert.ToBase64String(KeyDerivation.Pbkdf2( // pdKDF2 laskentafunktio
                password: newUser.Password, // eli otetaan käyttäjän salasana
                salt: salt, // suola mukana salasanassa
                prf: KeyDerivationPrf.HMACSHA256, // käytetään sha256
                iterationCount: 10000, // kuinka pitkaan algoritmia pyoritetaan, menee enemmän aikaa (parempi turvallisuudelle)
                numBytesRequested: 256 / 8)); // 256 bittinen arvo
            */
            newUser.UserName = user.UserName;
            newUser.FirstName = user.FirstName;
            newUser.LastName = user.LastName;
            /*
            newUser.Salt = salt;
            newUser.Password = hashedPassWord;
            */
            newUser.JoinDate = user.JoinDate;
            newUser.LoginDate = user.LoginDate;

            if (newUser == null)
            { // tarkistetaan
                return null;
            }
            return newUser;
        }

        // valmis
        private UserDTO UserToDTO(User user) // muutetaan Userista DTO:ksi
        {
            UserDTO dto = new UserDTO(); // tehdään uusi
            dto.UserName = user.UserName; // pistetään arvot
            dto.FirstName = user.FirstName;
            dto.LastName = user.LastName;
            dto.JoinDate = user.JoinDate;
            dto.LoginDate = user.LoginDate;
            return dto;
        }
    }
}
