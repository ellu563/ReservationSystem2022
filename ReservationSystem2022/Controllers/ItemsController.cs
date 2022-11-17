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
    public class ItemsController : ControllerBase
    {
        // private readonly ReservationContext _context; // päästään tietokantaan kiinni (vanha)
        private readonly IItemService _service;

        private readonly IUserAuthenticationService _authenticationService;

        public ItemsController(IItemService service, IUserAuthenticationService authenticationService) // ottaa vastaan ja muistiin
        {
            _service = service;
            _authenticationService = authenticationService;
        }

        // GET: api/Items
        /// <summary>
        /// Get all items from database
        /// </summary>
        /// <returns>list of items</returns>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ItemDTO>>> GetItems()
        {
            return Ok(await _service.GetItemsAsync()); // Ok=http 200
        }

        // GET: api/Items/5
        /// <summary>
        /// Get a single item based on id
        /// </summary>
        /// <param name="id">Id of item</param>
        /// <returns>Data for single item</returns>
        /// <response code="200">Returns the item</response>
        /// <response code="404">Item not found from database</response>
       
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ItemDTO>> GetItem(long id) // palauttaa itemDTO:n, haetaan id:n perusteella
        {
            var item = await _service.GetItemAsync(id); // kutsutaan serviceä.. joka kutsuu repositorya

            if (item == null)
            {
                return NotFound(); // jos käyttäjä kysyy sellaista itemiä mitä ei ole, http 404 ei löytynyt mitään
            }

            return item;
        }

        // PUT: api/Items/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize] // vaaditaan
        public async Task<IActionResult> PutItem(long id, ItemDTO item)
        {
            // katsotaan ensin onko kutsu ok
            if (id != item.Id) // onko osoitteen id ja itemin id samat, jos ei
            {
                return BadRequest(); // palauta badrequest
            }

            // tarkista, onko oikeus muokata
            bool isAllowed = await _authenticationService.IsAllowed(this.User.FindFirst(ClaimTypes.Name).Value, item);

            if(!isAllowed)
            {
                return Unauthorized();
            }

            ItemDTO updatedItem = await _service.UpdateItemAsync(item); // jos on sama, lähetetään eteenpäin servicelle
            if(updatedItem == null)
            {
                return NotFound();
            }
            return NoContent();

        }

        // POST: api/Items = tätä muokattu
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ItemDTO>> PostItem(ItemDTO item)
        {
            ItemDTO newItem = await _service.CreateItemAsync(item);
            if(newItem == null)
            {
                return Problem();
            }

            return CreatedAtAction("GetItem", new { id = newItem.Id }, newItem);
        }

        // DELETE: api/Items/5
        [HttpDelete("{id}")]
        [Authorize] // saa omat iteminsä poistaa
        public async Task<IActionResult> DeleteItem(long id)
        {
            if(await _service.DeleteItemAsync(id))
            {
                return Ok();
            }
            return NotFound();
        }

    }
}
