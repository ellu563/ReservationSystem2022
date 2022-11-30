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

    // tää on nyt muokattu samalla tavalla kun toi itemscontroller

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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations()
        {
            return Ok(await _service.GetReservationsAsync());
        }

        // GET: api/Reservations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationDTO>> GetReservation(long id) // 653
        {
            var reservation = await _service.GetReservationAsync(id); // 653
            // vanha: var reservation = await _context.Reservations.FindAsync(id);

            if (reservation == null)
            {
                return NotFound();
            }

            return reservation;
        }

        // PUT: api/Reservations/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
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
        [HttpPost]
        public async Task<ActionResult<ReservationDTO>> PostReservation(ReservationDTO reservation)
        {
            // tarkista, onko oikeus muokata
            bool isAllowed = await _authenticationService.IsAllowed(this.User.FindFirst(ClaimTypes.Name).Value, reservation);

            if (!isAllowed)
            {
                return Unauthorized();
            }

            ReservationDTO newReservation = await _service.CreateReservationAsync(reservation);

            if (newReservation == null)
            {
                return Problem();
            }

            return CreatedAtAction("GetItem", new { id = newReservation.Id }, newReservation);

            /* vanha, ylempi on oma muokattu itemscontrollerista
             
             _context.Reservations.Add(reservation);
             await _context.SaveChangesAsync();

             return CreatedAtAction("GetReservation", new { id = reservation.Id }, reservation);
            */
        }

        // DELETE: api/Reservations/5
        [HttpDelete("{id}")]
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
