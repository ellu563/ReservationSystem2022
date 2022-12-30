namespace ReservationSystem2022.Models
{
    // Varaus 
    public class Reservation
    {
        public long Id { get; set; }

        // kuka varauksen tekee "varauksen omistaja"
        // ? = voi olla myös tyhjä, eli voit nähdä pelkän varauksen ilman että käyttäjää enää on
        public virtual User? Owner { get; set; }

        // mitä varataan, mitä kohdetta tämä käyttäjä varaa
        // ? = voi olla myös tyhjä, sallitaan että on varaus jolla ei ole omistajaa tai kohdetta
        public virtual Item? Target { get; set; }

        // milloin varaus alkaa
        public DateTime StartTime { get; set; }

        // milloin varaus loppuu
        public DateTime EndTime { get; set; }

    }

    public class ReservationDTO
    {
        public long Id { get; set; }
        public String Owner { get; set; }
        public long Target { get; set; } // itemin id
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
