using Microsoft.EntityFrameworkCore;
using ReservationSystem2022.Models;
using ReservationSystem2022.Repositories;

namespace ReservationSystem2022.Services
{
    public class ItemService : IItemService
    {
        // (implement interface)

        // tarvitaan viittaus repositoryyn:
        public readonly IItemRepository _repository;
        private readonly IUserRepository _userRepository;

        // constructori
        public ItemService(IItemRepository repository, IUserRepository userRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
        }

        public async Task<ItemDTO> CreateItemAsync(ItemDTO dto)
        {
            Item newItem = await DTOToItem(dto);
            await _repository.AddItemAsync(newItem);
            return ItemToDTO(newItem);
        }

        public async Task<bool> DeleteItemAsync(long id)
        {
            Item oldItem = await _repository.GetItemAsync(id);
            if(oldItem == null)
            {
                return false;
            }
            return await _repository.DeleteItemAsync(oldItem);
        }

        public async Task<ItemDTO> GetItemAsync(long id)
        {
            Item item = await _repository.GetItemAsync(id); // ..service kutsuu repositorya

            if (item != null) // tarkistetaan löytyykö sieltä mitään
            {
                // Update access count
                item.accessCount++; // jos on löytynyt niin voidaan päivittää accesscounttia
                await _repository.UpdateItemAsync(item);

                return ItemToDTO(item); // jos löytyy niin tehdään siitä DTO
            }
            return null; // jos ei ole löydetty, palautetaan null
        }

        public async Task<IEnumerable<ItemDTO>> GetItemsAsync()
        {
            IEnumerable<Item> items = await _repository.GetItemsAsync(); // itemit tietokannasta
            List<ItemDTO> result = new List<ItemDTO>(); // lista dto:sta
            foreach (Item i in items) // käydään tietokannasta saatu items-lista läpi
            {
                result.Add(ItemToDTO(i)); // käydään jokainen item läpi, heitetään se ItemToDTO funktiolle, ja lopputulos lisätään listaan
            }
            return result; // palautetaan
        }

        public async Task<ItemDTO> UpdateItemAsync(ItemDTO item)
        {
            Item oldItem = await _repository.GetItemAsync(item.Id); // haetaan ensin tietokannasta (jos ei ole muistissa)
            // haetaan löytyykö vanhat tiedot vastaavalle itemille
            // id:n perusteella siis haetaan ja pistetään tiedot oldItemiin

            if(oldItem == null) // ei välttämättä löydy kyseistä, jos ei löydy tietokannasta ei ole mitään mitä muokata
            {
                return null;
            }
            oldItem.Name = item.Name; 
            oldItem.Description = item.Description;
            oldItem.Images = item.Images;
            oldItem.accessCount++;
            // eli olditem on haettu tietokannasta, ja siihen on kopioitu kaikki kenttien arvot
            // nyt voidaan se tallentaa
            Item updatedItem=await _repository.UpdateItemAsync(oldItem);
            if(updatedItem == null) // joku on mennyt vikaan
            {
                return null;
            }
            return ItemToDTO(updatedItem); // updateditem on käytänössä sama kuin oldItem, on vaan tehty se noin tarkastuksen vuoksi
            // eli tehdään siitä dto ja lähetetään controlleriin
        }

        // eli verkon yli on tullut ItemDTO, tämä funktio siis siirtää ne tiedot item -muotoiseksi, joka siis voidaan vaikka..
        // tallentaa tietokantaan
        private async Task<Item> DTOToItem(ItemDTO dto)
        {
            Item newItem = new Item(); // luodaan uusi itemi
            newItem.Name = dto.Name; // arvot, eli laitetaan dto:n tiedot uuteen itemiin
            newItem.Description = dto.Description;

            User owner = await _userRepository.GetUserAsync(dto.Owner);

            // owneria ei saadakkaan noin niinkun noi ylemmät, haetaan omistaja tietokannasta
            // User owner = _context.Users.Where(x => x.UserName == dto.Owner).FirstOrDefault();

            if (owner != null)
            {
                newItem.Owner = owner; // owner on nyt käyttäjä joka löydettiin
            }

            newItem.Images = dto.Images;
            newItem.accessCount = 0;
            return newItem;
        }

        // toisinpäin: eli halutaan lukea yhden Itemin tiedot, asiakas esim. kysyy jonkun itemin tietoja,
        // tietokannasta luetaan yhden Itemin tiedot, mutta ennen verkon yli
        // lähetystä se pitää muuttaa itemDTO:ksi
        private ItemDTO ItemToDTO(Item item)
        { 
            ItemDTO dto = new ItemDTO();
            dto.Id = item.Id;
            dto.Name = item.Name;
            dto.Description = item.Description;
            dto.Images = item.Images;
            if(item.Owner != null)
            {
                dto.Owner = item.Owner.UserName;
            }
            return dto;
        }
    }
}
