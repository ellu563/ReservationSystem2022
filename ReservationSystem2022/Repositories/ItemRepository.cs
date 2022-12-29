using Microsoft.EntityFrameworkCore;
using ReservationSystem2022.Models;
using System.Net;

namespace ReservationSystem2022.Repositories
{
    // tietokantaa käsitellään ainoastaan repository tasolla, hakee datan ja tallentaan datan tietokantaan
    public class ItemRepository : IItemRepository // toteuttaa rajapinnan IItemRepository
    {
        // tarvitaan viittaus tietokantaan
        private readonly ReservationContext _context;

        public ItemRepository(ReservationContext context)
        {
           _context = context;
        }

        // tallennetaan item tietokantaan/saadaan lisättyä uusia itemejä
        public async Task<Item?> AddItemAsync(Item item)
        {
            _context.Items.Add(item); // lisätään kokoelmaan (muistissa), ei vielä tallennettu tietokantaan 
            try
            {
                await _context.SaveChangesAsync(); //  tallennetaan/contextin tilan päivitys (kutsutaan asynkronista = await)
            }
            catch(Exception e)
            {
                return null;
            }
            return item;
        }

        // kuvien poistoa varten
        public async Task<bool> ClearImages(Item item)
        {
            if(item != null) // onko kuvia
            {
                // käydään kaikki kuvat läpi
                foreach(Image i in item.Images)
                {
                    _context.Images.Remove(i); // poistetaan se kuva
                }
                // tallennetaan muutokset
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch(Exception ex)
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> DeleteItemAsync(Item item)
        {
            try
            {
                _context.Items.Remove(item); // poistetaan (muistista)
                await _context.SaveChangesAsync(); // saadaan päivitettyä tietokantaan myös tieto
            }
            catch(Exception ex)
            {
                return false;
            }
            return true;
        }

        public async Task<Item> GetItemAsync(long id) // hakee id:n perusteella vain yhden
        {
            // palauttaa tietokannasta löytyvä itemi
            return await _context.Items.Include(i => i.Images).FirstOrDefaultAsync(i => i.Id == id); // löytyykö tietokannasta ja palauttaa
            // haetaan myös kuvat Includella
        }

        public async Task<IEnumerable<Item>> GetItemsAsync() // hakee kaikki
        {
            return await _context.Items.Include(i => i.Images).ToListAsync(); // hakee koko listan sisällön (ja imaget)
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(User user)
        {
            return await _context.Items.Include(i => i.Owner).Where(x => x.Owner == user).ToListAsync();
        }

        public async Task<IEnumerable<Item>> QueryItems(string query)
        {
            return await _context.Items.Include(i => i.Owner).Where(x => x.Name.Contains(query)).ToListAsync();
        }

        public async Task<Item> UpdateItemAsync(Item item) //suurinosa toiminnoista service tasolla
        // eli service huolehtii että kopioidaan muuttuneet tiedot,ja repository katsoo että muutokset 
        // tallennetaan
        {
            try
            {
                await _context.SaveChangesAsync(); // tallennus
            }
            catch(Exception ex)
            {
                return null; // muutosten tallentaminen ei onnistunut
            }
            return item; // "tallennus on onnistunut"
        }
    }
}
