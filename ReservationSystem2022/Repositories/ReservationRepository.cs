using ReservationSystem2022.Models;

namespace ReservationSystem2022.Repositories
{
    public class ReservationRepository : IReservationRepository // pistetään toi perimisluokka, ja implement interfacella saahaa toi og sisältö
    {
        public Task<Reservation> AddReservationAsync(Reservation reservation)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteReservationAsync(Reservation reservation)
        {
            throw new NotImplementedException();
        }

        public Task<Reservation> GetReservationAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Reservation>> GetReservationAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Reservation> UpdateReservationAsync(Reservation reservation)
        {
            throw new NotImplementedException();
        }
    }
}
