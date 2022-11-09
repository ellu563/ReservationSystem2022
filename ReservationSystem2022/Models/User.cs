namespace ReservationSystem2022.Models
{
    public class User
    // käyttäjä
    {
        public long Id { get; set; }
        public String UserName { get; set; }
        public String Password { get; set; }
        public String? FirstName { get; set; }
        public String? LastName { get; set; }
        public DateTime JoinDate { get; set; }
        public DateTime LoginDate { get; set; }
    }
}
