using System.ComponentModel.DataAnnotations;

namespace ReservationSystem2022.Models
{
    public class Item
    // varattava kohde
    {
        // eli mm. tämän mallin mukaan rakentuu tietokanta, nämä tiedot tallennetaan tietokantaan
        public long Id { get; set; }
        public String Name { get; set; }
        public String? Description { get; set; }
        
        //tieto käyttäjästä (kuka on luonut)
        public virtual User? Owner { get; set; }
        public virtual List<Image>? Images { get; set; }

        public long accessCount { get; set; } // kuinka monta kertaa tätä itemiä on luettu
        // huom. uusi kenttä lisätty migraation avulla tietokantaan
        // dotnet ef migrations add AddAccessCount, dotnet ed database update
    }

    // ItemDTO = nämä tiedot lähetetään verkon yli, ei välttämättä haluta kaikkia ylläolevia tietoja haluta lähettää verkon yli
    // joten valitaan ne erikseen tähän (verkon yli siirretään minimäärä tietoa)
    // osa tiedoista esim. järjestelmän sisäisiä, kuten tuo accesCount, ei järkeä siirtää verkon yli
    // siirto-objekti = dto / data transfer object
    // voi olla myös yhdistelmä eri tietokannan tauluista, ei välttämättä pakko tehdä tolleen vain yhdestä
    public class ItemDTO
    {
        public long Id { get; set; }

        // tietojen validointi, nämä säännöt kuuluvat nyt vain verkon yli lähetettäville
        // rajoitukset kirjoitetaan aina kentän nimeen eteen annotaatiolla [], tässä tapauksessa ne siis kuuluvat Namelle
        // kontrolleri tarkistaa rajoitukset DTO-luokasta. palauttaa http virheen jos tiedot vääränlaisia
        // kontrolleri tarkastaa automaattisesti, jossa [apicontroller], nuo säännöt siis pelkästään riittää
        [Required] // pakollinen, pitää olla arvo
        [MinLength(4)] // tähän voitaisiin myös kirjottaa oma virheviesti perään (4,ErrorMessage="Too short")]
        [MaxLength(50)]
        public String Name { get; set; } 
        public String? Description { get; set; } // voisi laittaa myös esim. oletusarvon get; set } = "Default value";
        [Required]
        public String? Owner { get; set; } // omistajan käyttäjänimi, ei tarvitse nyt olla User kun tulee verkon yli
        public virtual List<ImageDTO>? Images { get; set; } // haetaan imageDTO

    }
}
