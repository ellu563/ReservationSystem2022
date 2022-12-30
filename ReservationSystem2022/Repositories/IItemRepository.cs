using ReservationSystem2022.Models;

namespace ReservationSystem2022.Repositories
{
    public interface IItemRepository
    {
        public Task<Item> GetItemAsync(long id); // get yhdelle
        public Task<IEnumerable<Item>> GetItemsAsync(); // get koko listalle

        // tassakii haetaa usernamen perusteella talleen
        public Task<IEnumerable<Item>> GetItemsAsync(User user);

        public Task<IEnumerable<Item>> QueryItems(String query);
        public Task<Item> AddItemAsync(Item item); 
        public Task<Item> UpdateItemAsync(Item item);
        public Task<Boolean> DeleteItemAsync(Item item);

        public Task<Boolean> ClearImages(Item item); // miltä itemiltä poistetaan kuvat
    }
}
