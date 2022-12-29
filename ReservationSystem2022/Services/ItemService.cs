using Microsoft.EntityFrameworkCore;
using ReservationSystem2022.Models;
using ReservationSystem2022.Repositories;
using System.Transactions;

namespace ReservationSystem2022.Services
{
    public class ItemService : IItemService
    {
        // service = toimintalogiikka

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
            // poistetaan myös kuvat
            await _repository.ClearImages(oldItem);
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
            
            // onko kuvia tietokannassa, poistetaan vaan jos siellä on vanhoja kuvia, ja on tulossa uusia kuvia
            if(oldItem.Images != null && item.Images != null)
            {
                await _repository.ClearImages(oldItem); // poistetaan vanhat kuvat jotta voidaan laittaa uudet
            }
            // uudet tiedot, onko siellä kuvia
            if(item.Images != null)
            {
                oldItem.Images = new List<Image>();
                foreach(ImageDTO i in item.Images)
                {
                    Image image = DTOToImage(i);
                    image.Target = oldItem;
                    oldItem.Images.Add(image);
                }
            }

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

        public async Task<IEnumerable<ItemDTO>> QueryItemsAsync(String query)
        {
           IEnumerable<Item> items = await _repository.QueryItems(query);
           List<ItemDTO> itemDTOs = new List<ItemDTO>();
            foreach(Item i in items)
            {
                itemDTOs.Add(ItemToDTO(i));
            }
            return itemDTOs;
        }

        // usernamea varten
        public async Task<IEnumerable<ItemDTO>> GetItemsAsync(string username)
        {
            User owner = await _userRepository.GetUserAsync(username);
            if(owner == null)
            {
                return null;
            }
            IEnumerable<Item> items = await _repository.GetItemsAsync(owner);
            List<ItemDTO> itemDTOs=new List<ItemDTO>();
            foreach(Item i in items)
            {
                itemDTOs.Add(ItemToDTO(i));
            }
            return itemDTOs;
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
            // onko dto:ssa kuvia
            if(dto.Images != null)
            {
                newItem.Images = new List<Image>(); // uusi lista kuvia
                // lisätään kuvat listaan, yksi tai useampi
                foreach(ImageDTO i in dto.Images)
                {
                    newItem.Images.Add(DTOToImage(i));
                }
            }

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
            // onko kuvia
            if(item.Images != null)
            {
                dto.Images = new List<ImageDTO>();
                foreach(Image i in item.Images)
                {
                    dto.Images.Add(ImageToDTO(i));
                }
            }
            if(item.Owner != null)
            {
                dto.Owner = item.Owner.UserName;
            }
            return dto;
        }

        // imagen ja dto:n väliset muutokset
        private Image DTOToImage(ImageDTO dto)
        {
            Image image = new Image(); // luodaan
            image.Url = dto.Url;
            image.Description = dto.Description;
            return image;
        }
        // image imageDTO:ksi
        private ImageDTO ImageToDTO(Image image)
        {
            ImageDTO dto = new ImageDTO();
            dto.Url = image.Url;
            dto.Description = image.Description;
            return dto;
        }

    }
}
