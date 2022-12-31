using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationSystem2022.Middleware;
using ReservationSystem2022.Models;
using ReservationSystem2022.Services;

namespace ReservationSystem2022.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        // controller luodaan aina controllers kansioon, add new scaffolded item.. api controller with actions.. tähän valittu model: user
        private readonly IUserService _service;
        private readonly IUserAuthenticationService _authenticationService;

        public UsersController(IUserService service, IUserAuthenticationService authenticationService)
        {
            _service = service;
            _authenticationService = authenticationService;
        }

        // GET: api/Users
        /// <summary>
        /// Get all users from the system
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers() 
        {
            return Ok(await _service.GetUsersAsync());
        }

        // GET: api/Users/user/username
        /// <summary>
        /// Gets users info based on username
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpGet("user/{userName}")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDTO>> GetUser(string userName)
        {
            var user = await _service.GetUserAsync(userName);

            if (user == null)
            {
                return NotFound();
            }
            return user;
        }

        // GET: api/Users/5
        /// <summary>
        /// Gets user by id number
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDTO>> GetUserById(long id)
        {
            var user = await _service.GetUserIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }
            return user;
        }

        // PUT: api/Users/user/username
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// You can update your users info, search by api/users/user/username
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut("user/{userName}")]
        [Authorize]
        public async Task<IActionResult> PutUser(string userName, User user)
        {
            if (userName != user.UserName)
            {
                return BadRequest();
            }

            // tarkista, onko oikeus muokata
            bool isAllowed = await _authenticationService.IsAllowed(this.User.FindFirst(ClaimTypes.Name).Value, user); 
            // huom; user, eli kun isallowedeja oli montaa eri parametreillä

            if (!isAllowed)
            {
                return Unauthorized();
            }
            
            UserDTO updatedUser = await _service.UpdateUserAsync(user); // jos on sama, lähetetään eteenpäin servicelle
            if (updatedUser == null)
            {
                return NotFound();
            }
            

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Post a new User
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<UserDTO>> PostUser(User user)
        {
            // tarkista, onko oikeus muokata
            bool isAllowed = await _authenticationService.IsAllowed(this.User.FindFirst(ClaimTypes.Name).Value, user);

            if (!isAllowed)
            {
                return Unauthorized();
            }

            UserDTO dto = await _service.CreateUserAsync(user); // eli täällä nyt käytetään tuota servicessä olevaa CreateUserAsync

            if(dto == null)
            {
                return Problem(); // tallennuksessa menee joku vikaan
            }

            return CreatedAtAction(nameof(PostUser), new { username = dto.UserName }, dto); // eli palautetaan userDTO:n tiedot
            // eli lähetään postilla: username, password, firstname, lastname
            // mutta ei nähdä sit siellä esim. postmanin alhaalla "palautuksessa" sitä passwordia
        }

        // DELETE: api/Users/id
        /// <summary>
        /// Delete your user by id number
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(long id)
        {
            User userDel = await _service.GetIdAsync(id);
            // poistetaan nyt vaan suoraan kannasta kayttaen Useria (ei userDTO:ta)
            userDel.Id = id;

            // tarkista, onko oikeus muokata
            bool isAllowed = await _authenticationService.IsAllowed(this.User.FindFirst(ClaimTypes.Name).Value, userDel);

            if (!isAllowed)
            {
                return Unauthorized();
            }

            if (await _service.DeleteUserAsync(id))
            {
                return Ok();
            }
            return NotFound();
        }

    }
}
