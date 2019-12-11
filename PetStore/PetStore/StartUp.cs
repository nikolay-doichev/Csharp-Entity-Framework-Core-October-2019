namespace PetStore
{
    using PetStore.Data;
    using PetStore.Services.Implementations;
    using System;

    public class StartUp
    {
        public static void Main()
        {
            using (var data = new PetStoreDbContext())
            {
                var userService = new UserService(data);
                var foodService = new FoodService(data, userService);
                var toyService = new ToyService(data, userService);

                //foodService.BuyFromDistributor("Cat Food", 0.350, 1.10m, 0.3, DateTime.Now, 1, 1);

                //var toyService = new ToyService(data);

                //toyService.BuyFromDistributor("Ball", null, 3.5m, 0.3, 1, 1);

                //userService.Register("Pesho", "pesho123@mail.com");
                //foodService.SellFoodToUser(1, 1);

                toyService.SellToyToUser(1, 1);
            }
        }
    }
}
