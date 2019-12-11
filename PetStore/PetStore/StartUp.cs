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
                var breedService = new BreedService(data);
                var categoryService = new CategoryService(data);
                var petService = new PetService(data, breedService, categoryService, userService);


                petService.BuyPet(Models.Enumerations.Gender.Male, DateTime.Now, 0m, null, 1, 1);

                petService.SellPet(1, 1);
            }
        }
    }
}
