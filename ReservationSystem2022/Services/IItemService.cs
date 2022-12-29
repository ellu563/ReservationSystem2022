using ReservationSystem2022.Models;

namespace ReservationSystem2022.Services
{
    public interface IItemService
    {
        // saa itemDTO:n, palauttaa kontrolleriin itemdton
        public Task<ItemDTO> CreateItemAsync(ItemDTO dto);
        public Task<ItemDTO> GetItemAsync(long id);
        public Task<IEnumerable<ItemDTO>> GetItemsAsync();

        public Task<IEnumerable<ItemDTO>> GetItemsAsync(String username);

        public Task<IEnumerable<ItemDTO>> QueryItemsAsync(String query);

        // ottaa itemdton vastaan
        public Task<ItemDTO> UpdateItemAsync(ItemDTO item);

        // ottaa id:n vastaan
        public Task<Boolean> DeleteItemAsync(long id);
    }
}
