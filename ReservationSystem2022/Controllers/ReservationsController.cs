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

    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _service;
        private readonly IUserAuthenticationService _authenticationService;

        public ReservationsController(IReservationService service, IUserAuthenticationService authenticationService)
        {
            _service = service;
            _authenticationService = authenticationService;
        }

        // GET: api/Reservations
        /// <summary>
        /// Get all the reservations
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations()
        {
            return Ok(await _service.GetReservationsAsync());
        }

        // GET: api/Reservations/5
        /// <summary>
        /// Gets your spesific reservation using id number
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<ReservationDTO>> GetReservation(long id) 
        {
            var reservation = await _service.GetReservationAsync(id); 

            if (reservation == null)
            {
                return NotFound();
            }

            return reservation;
        }

        // PUT: api/Reservations/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Update your reservation using id number
        /// </summary>
        /// <param name="id"></param>
        /// <param name="reservation"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutReservation(long id, ReservationDTO reservation)
        {
            if (id != reservation.Id)
            {
                return BadRequest();
            }

            // tarkista, onko oikeus muokata
            bool isAllowed = await _authenticationService.IsAllowed(this.User.FindFirst(ClaimTypes.Name).Value, reservation);
            // ja täällä se on reservation (isallowed funktiota kun oli 3)

            if (!isAllowed)
            {
                return Unauthorized();
            }

            // oma: 
            ReservationDTO updatedReservation = await _service.UpdateReservationAsync(reservation); // jos on sama, lähetetään eteenpäin servicelle
            if (updatedReservation == null)
            {
                return NotFound();
            }
            return NoContent();
        }

        /* vanha koodi: 
        _context.Entry(reservation).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ReservationExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
        }*/


        // POST: api/Reservations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Post a new reservation
        /// </summary>
        /// <param name="reservation"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ReservationDTO>> PostReservation(ReservationDTO reservation)
        {
            // tarkista, onko oikeus muokata
            bool isAllowed = await _authenticationService.IsAllowed(this.User.FindFirst(ClaimTypes.Name).Value, reservation);

            if (!isAllowed)
            {
                return Unauthorized();
            }

            reservation = await _service.CreateReservationAsync(reservation);

            if (reservation == null)
            {
                return BadRequest();
            }

            return CreatedAtAction("GetReservation", new { id = reservation.Id }, reservation);

            /* vanha
             
             _context.Reservations.Add(reservation);
             await _context.SaveChangesAsync();

             return CreatedAtAction("GetReservation", new { id = reservation.Id }, reservation);
            */
        }

        // DELETE: api/Reservations/5
        /// <summary>
        /// Delete your reservation using id number
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteReservation(long id)
        {
            ReservationDTO reservation = new ReservationDTO();
            reservation.Id = id;

            // tarkista, onko oikeus muokata
            bool isAllowed = await _authenticationService.IsAllowed(this.User.FindFirst(ClaimTypes.Name).Value, reservation);

            if (!isAllowed)
            {
                return Unauthorized();
            }

            if (await _service.DeleteReservationAsync(id))
            {
                return Ok();
            }
            return NotFound();

            /* vanha
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return NoContent();
             */
        }

    }
}
