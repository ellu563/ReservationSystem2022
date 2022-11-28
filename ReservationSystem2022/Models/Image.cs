namespace ReservationSystem2022.Models
{
    public class Image
    // Kuva
    {
        public long Id { get; set; }
        public String? Description { get; set; }
        public String Url { get; set; }

        // mihin itemiin liittyy, samalla itemillä voi olla useampi kuva
        public virtual Item Target { get; set; }
    }

    public class ImageDTO
    {
        public String? Description { get; set; }
        public String Url { get; set; }
    }
}
