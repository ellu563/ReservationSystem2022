using ReservationSystem2022.Models;

namespace ReservationSystem2022.Services
{
    public class ReservationService : IReservationService
    {
        public Task<ReservationDTO> CreateReservationAsync(ReservationDTO dto)
        {
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
    }
}
