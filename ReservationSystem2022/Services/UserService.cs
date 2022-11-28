using Microsoft.AspNetCore.Cryptography.KeyDerivation;
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
