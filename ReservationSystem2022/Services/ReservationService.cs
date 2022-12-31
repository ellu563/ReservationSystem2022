using NuGet.Configuration;
using NuGet.Protocol.Core.Types;
using ReservationSystem2022.Models;
using ReservationSystem2022.Repositories;
using System.Linq.Expressions;

namespace ReservationSystem2022.Services
{
    public class ReservationService : IReservationService
    {
        public readonly IReservationRepository _repository;
        private readonly IUserRepository _userRepository;
        public readonly IItemRepository _itemRepository;
        

        public ReservationService(IReservationRepository repository, IUserRepository userRepository, IItemRepository itemRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
            _itemRepository = itemRepository;
        }

        // luo uusi
        public async Task<ReservationDTO> CreateReservationAsync(ReservationDTO dto)
        {
            if(dto.StartTime >= dto.EndTime)
            {
                return null;
            }
            // onko item olemassa
            Item target = await _itemRepository.GetItemAsync(dto.Target);

            // jos ei loydy ei voida tehda varausta
            if(target == null) {
                return null;
            }
            // onko silla aikavalilla varauksia, toivotaan siis etta ei ole yhtaan
            IEnumerable<Reservation> reservations = await _repository.GetReservationsAsync(target, dto.StartTime, dto.EndTime);
            if(reservations.Count() > 0)
            {
                return null;
            }
            Reservation newReservation = await DTOToReservationAsync(dto);
            newReservation = await _repository.AddReservationAsync(newReservation);

            return ReservationToDTO(newReservation);
        }

        // hae id:n perusteella
        public async Task<ReservationDTO> GetReservationAsync(long id)
        {
            Reservation res = await _repository.GetReservationAsync(id); 

            if (res != null) 
            {
                return ReservationToDTO(res); 
            }
            return null; 
        }

        // hae kaikki reservationit
        public async Task<IEnumerable<ReservationDTO>> GetReservationsAsync()
        {
            IEnumerable<Reservation> reservations = await _repository.GetReservationsAsync(); 
            List<ReservationDTO> result = new List<ReservationDTO>(); 
            foreach (Reservation i in reservations) 
            {
                result.Add(ReservationToDTO(i)); 
            }
            return result; // palautetaan
        }

        // paivitys
        public async Task<ReservationDTO> UpdateReservationAsync(ReservationDTO reservation)
        {
            Reservation oldRes = await _repository.GetReservationAsync(reservation.Id); 
            // haetaan id:n perusteella

            if (oldRes == null) // ei välttämättä löydy kyseistä, jos ei löydy tietokannasta ei ole mitään mitä muokata
            {
                return null;
            }
            oldRes.Id = reservation.Id;
           
            // huom. tassa vissiin joku vika, kun tietokantaan osaa paivittaa esim. paivamaaran muutoksen
            // mutta targettia ei osaa paivittaa, eika osaa myoskaan hakea get kutsulla oikeaa target numeroa
            // vaikka se oikea target numero kylla menee tietokantaan ja nakyy siella
            // vain postman get kutsussa ei nay, eika tassa updatessa
            User owner = await _userRepository.GetUserAsync(reservation.Owner);
            Item target = await _itemRepository.GetItemAsync(reservation.Target);

            if (owner == null)
            {
                return null;
            }
            oldRes.Owner = owner;

            if (target != null)
            {
                return null;
            }
            oldRes.Target = target;
            // ..? 

            oldRes.StartTime = reservation.StartTime;
            oldRes.EndTime = reservation.EndTime;

            // nyt kun kaikkiin kenttiin tallennettu uusi arvo niin updatetaan se tieto
            Reservation updatedRes = await _repository.UpdateReservationAsync(oldRes);
            if (updatedRes == null) // joku on mennyt vikaan
            {
                return null;
            }
            return ReservationToDTO(updatedRes);
        }

        // poisto
        public async Task<bool> DeleteReservationAsync(long id)
        {
            Reservation oldRes = await _repository.GetReservationAsync(id);
            if (oldRes == null)
            {
                return false;
            }
            return await _repository.DeleteReservationAsync(oldRes);
        }

        // dto muunnos
        private async Task<Reservation> DTOToReservationAsync(ReservationDTO dto)
        {
            Reservation newReservation = new Reservation();
            newReservation.Id = dto.Id;
            
            User owner = await _userRepository.GetUserAsync(dto.Owner);
            Item target = await _itemRepository.GetItemAsync(dto.Target);
            newReservation.StartTime = dto.StartTime;
            newReservation.EndTime = dto.EndTime;

            if (owner != null) 
            {
                newReservation.Owner = owner; // owner on nyt käyttäjä joka löydettiin
            }

            if(target != null)
            {
                newReservation.Target = target;
            }

            return newReservation;
        }
        
        // muunnos
        private ReservationDTO ReservationToDTO(Reservation res)
        {
            ReservationDTO dto = new ReservationDTO();

            dto.Id = res.Id;
            if(res.Owner != null)
            {
                dto.Owner = res.Owner.UserName;
            }
            // huom. tassa on viela joku ongelma etta se nayttaa nollaa
            // menee kantaan oikein mutta ei nayta postmanin get kutsussa oikeaa numeroa
            if (res.Target != null)
            {
                dto.Target = res.Target.Id;
            }
            
            dto.StartTime = res.StartTime;
            dto.EndTime = res.EndTime;

            return dto;
        }

    }
}
