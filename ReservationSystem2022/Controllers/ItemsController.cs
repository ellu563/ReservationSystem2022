﻿using System;
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
        
        // eli controller ottaa vastaan verkon yli saapuvat kutsut ja  kutsuu tarvittavia palveluita service luokasta

        // oma kommentti: eli program.cs:ssä on esitelty mikä interface toteuttaa mikä luokka, ja näin päästään kiinni suoraan 
        // siihen ns. luokkaan, tai vaikka useampaan luokkaan,
        // ei siis sinällään väliä mitä siellä luokassa toteutetaan, interface vaan ns. suorittaa sen luokan toteutuksen/tai voi ajatella myös toisinpäin
        // eli kaikki eri paikat voi käyttää sitä, kun se esitellään tuolleen interfacena, ja esitelty program.cs
        // nyt kun on vain yksi luokka siellä, se interface siis "toteuttaa" sen luokan
        // ja viiään se myös tuonne constructoriin.. tässä siis päästään kiinni sinne userauthenticationserviceen

        private readonly IUserAuthenticationService _authenticationService;

        public ItemsController(IItemService service, IUserAuthenticationService authenticationService) // ottaa vastaan ja muistiin
        {
            _service = service;
            _authenticationService = authenticationService;
        }

        // GET: api/Items
        /// <summary>
        /// Get all items from the system
        /// </summary>
        /// <returns>list of items</returns>
        [HttpGet]
        [Authorize] // kaikki kayttajat ketka mennyt autentikoinnin lapi voivat kayttaa 
        public async Task<ActionResult<IEnumerable<ItemDTO>>> GetItems()
        {
            return Ok(await _service.GetItemsAsync()); // Ok=http 200
        }

        // haetaan usernamen perusteella
        // GET: api/Items/user/username
        /// <summary>
        /// Get all items from the system matching user username
        /// </summary>
        /// <returns>list of items</returns>
        [HttpGet("user/{username}")]
        [Authorize]
        public async Task<ActionResult<ItemDTO>> GetItems(String username)
        {
            return Ok(await _service.GetItemsAsync(username));
        }

        // lisä: haetaan hakutermin perusteella
        // GET: api/Items/query
        /// <summary>
        /// Get all items from the system matching given query
        /// </summary>
        /// <returns>list of items</returns>
        [HttpGet("{query}")]
        [Authorize]
        public async Task<ActionResult<ItemDTO>> QueryItems(String query)
        {
            return Ok(await _service.QueryItemsAsync(query));
        }

        // GET: api/Items/5
        /// <summary>
        /// Get a single item based on id
        /// </summary>
        /// <param name="id">Id of item</param>
        /// <returns>Data for single item</returns>
        /// <response code="200">Returns the item</response>
        /// <response code="404">Item not found from database</response>

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<ItemDTO>> GetItem(long id) // palauttaa itemDTO:n, haetaan id:n perusteella
        {
            var item = await _service.GetItemAsync(id); // kutsutaan serviceä.. joka kutsuu repositorya
            // await = async, eli säikeen voi vapauttaa muihin töihin

            if (item == null)
            {
                return NotFound(); // jos käyttäjä kysyy sellaista itemiä mitä ei ole, http 404 ei löytynyt mitään
            }

            return item;
        }

        // PUT: api/Items/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Updates your item based on id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize] // vaaditaan
        public async Task<IActionResult> PutItem(long id, ItemDTO item)
        {
            // katsotaan ensin onko kutsu ok
            if (id != item.Id) // onko osoitteen id ja itemin id samat, jos ei
            {
                return BadRequest(); // palauta badrequest
            }

            // tarkista, onko oikeus muokata, tarvitaan sen käyttäjätunnus ja päästään siihen käsiksi tuolla claimilla
            // eli käytännössä on oikeus muokata oma tekemää itemiä (put = muokkaus)
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

        // POST: api/Items
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Posts a new item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ItemDTO>> PostItem(ItemDTO item)
        {
            // tarkista, onko oikeus muokata
            bool isAllowed = await _authenticationService.IsAllowed(this.User.FindFirst(ClaimTypes.Name).Value, item);

            if (!isAllowed)
            {
                return Unauthorized();
            }

            ItemDTO newItem = await _service.CreateItemAsync(item);
            if(newItem == null)
            {
                return Problem();
            }

            return CreatedAtAction("GetItem", new { id = newItem.Id }, newItem);
        }

        // DELETE: api/Items/5
        /// <summary>
        /// Deletes item using id number
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize] // saa omat iteminsä poistaa
        public async Task<IActionResult> DeleteItem(long id)
        {
            ItemDTO item = new ItemDTO();
            item.Id = id;

            // tarkista, onko oikeus muokata
            bool isAllowed = await _authenticationService.IsAllowed(this.User.FindFirst(ClaimTypes.Name).Value, item);

            if (!isAllowed)
            {
                return Unauthorized();
            }

            if (await _service.DeleteItemAsync(id))
            {
                return Ok();
            }
            return NotFound();
        }

    }
}
