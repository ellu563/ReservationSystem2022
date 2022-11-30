using NuGet.Protocol.Core.Types;
using ReservationSystem2022.Models;
using ReservationSystem2022.Repositories;

namespace ReservationSystem2022.Services
{
    public class ReservationService : IReservationService
    {
        public readonly IReservationRepository _reservationRepository;
        public readonly IItemRepository _itemRepository;
        private readonly IUserRepository _userRepository;

        public ReservationService(IReservationRepository repository, IUserRepository userRepository, IItemRepository itemRepository)
        {
            _reservationRepository = repository;
            _userRepository = userRepository;
            _itemRepository = itemRepository;
        }

        public Task<ReservationDTO> CreateReservationAsync(ReservationDTO dto)
        {
            /*
            Reservation newReservation = await DTOToReservation(dto);
            await _repository.AddItemAsync(newReservation);
            return ReservationToDTO(newReservation);
            */
            throw new NotImplementedException();
        }

        public Task<bool> DeleteReservationAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<ReservationDTO> GetReservationAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ReservationDTO>> GetReservationsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ReservationDTO> UpdateReservationAsync(ReservationDTO reservation)
        {
            throw new NotImplementedException();
        }


        // muokattu dto-to-itemin perusteella, ei ainakaa näytä erroria
        private async Task<Reservation> DTOToReservation(ReservationDTO dto)
        {
            Reservation newReservation = new Reservation(); 

            // pitää laittaa näin eikä niin ku siellä itemdto:ssa kun nää erilaisia
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

        // toisinpäin, muokattu myös taskiksi entiiä pitääkö KESKEN
        
        private async Task<ReservationDTO> ReservationToDTO(Reservation reservation)
        {
            ReservationDTO dto = new ReservationDTO();

            dto.Id = reservation.Id;
            dto.Owner = reservation.Owner.UserName;

            /* kesken: tunti jäi tähän, 30.11. muokattu

            if (owner != null)
            {
                dto.Owner = owner; // owner on nyt käyttäjä joka löydettiin
            }

            dto.Name = reservation.Name;
            dto.Description = reservation.Description;

            if (reservation.Owner != null)
            {
                dto.Owner = item.Owner.UserName;
            } */
            return dto;
        }
    }
}
