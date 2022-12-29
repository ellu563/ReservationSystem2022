using ReservationSystem2022.Models;

namespace ReservationSystem2022.Repositories
{
    public interface IReservationRepository // tää on sama kuin toi itemin, mut vaihettu vaan noi reservationit Item ja itemien tilalle,
    // ja tää IreservationRepo on tehty ensin, ja sit saahaa tehtyy toi ReservationRepo ja implement interface
    // interface tehdään jotta interfacea voidaan käyttää esim. itemscontrollerissa, jossa se otetaan vastaan, ja tehtyä siellä olio ja esiteltyä constructorissa
    // pitää olla esitelty program.cs:ssä mitä palveluita on ja sieltä osataan heittää viittaus sinne mistä sitä pyydetään
    { 
        public Task<Reservation> GetReservationAsync(long id); // get yhdelle
        public Task<IEnumerable<Reservation>> GetReservationsAsync(); // get koko listalle
        public Task<IEnumerable<Reservation>> GetReservationsAsync(Item target, DateTime start, DateTime end);

        public Task<Reservation> AddReservationAsync(Reservation reservation);
        public Task<Reservation> UpdateReservationAsync(Reservation reservation);
        public Task<Boolean> DeleteReservationAsync(Reservation reservation);
    }
}
