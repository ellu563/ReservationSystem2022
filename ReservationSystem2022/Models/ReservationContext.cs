using Microsoft.EntityFrameworkCore;

namespace ReservationSystem2022.Models
{
    public class ReservationContext : DbContext
    {
        // reservationcontext = luokka minkä kautta päästään tietokantaan käsiksi
        // saman kontekstin kautta pääsee jokaiseen kokoelmaan käsiksi
        public ReservationContext(DbContextOptions<ReservationContext> options)
            : base(options)
        {
        }
        public DbSet<Item> Items { get; set; } = null!;
        public DbSet<Image> Images { get; set; } = null!;
        public DbSet<Reservation> Reservations { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

    }
}
