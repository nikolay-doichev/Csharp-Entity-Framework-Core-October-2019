using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetStore.Data;
using PetStore.Models;
using PetStore.Models.Enumerations;
using PetStore.Services.Models.Food;

namespace PetStore.Services.Implementations
{
    public class FoodService : IFoodService
    {
        private readonly PetStoreDbContext data;
        private readonly IUserService userService;
        public FoodService(PetStoreDbContext data, IUserService userService)
        {
            this.data = data;
            this.userService = userService;
        }
        public void BuyFromDistributor(string name, 
            double weight, decimal price,double profit ,DateTime 
            expirationDate, int brandId, int categoryId)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or whitespace!");
            }

            if (profit < 0 || profit > 5)
            {
                throw new ArgumentException("Profit must be higher than zero and lower than 500%");
            }

            var food = new Food()
            {
                Name = name,
                Weight = weight,
                Price = price +(price * (decimal)profit),
                DistributorPrice = price,
                ExpirationDate = expirationDate,
                BrandId = brandId,
                CategoryId = categoryId
            };

            this.data.Food.Add(food);
            this.data.SaveChanges();
        }

        public void BuyFromDistributor(AddingFoodServiceModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                throw new ArgumentException("Name cannot be null or whitespace!");
            }

            if (model.Profit < 0 || model.Profit > 5)
            {
                throw new ArgumentException("Profit must be higher than zero and lower than 500%");
            }

            var food = new Food()
            {
                Name = model.Name,
                Weight = model.Weight,
                DistributorPrice = model.Price,
                Price = model.Price + (model.Price * (decimal)model.Profit),
                BrandId = model.BrandId,
                CategoryId = model.CategoryId
            };

            this.data.Food.Add(food);
            this.data.SaveChanges();
        }

        public bool Exist(int foodId)
        {
            return this.data.Food.Any(f => f.Id == foodId);
        }

        public void SellFoodToUser(int foodId, int userId)
        {
            if (!this.Exist(foodId))
            {
                throw new ArgumentException("There is no such food given Id in the database!");
            }

            if (!this.userService.Exist(userId))
            {
                throw new ArgumentException("There is no such user given Id in the database!");
            }

            var order = new Order()
            {
                PurchaseDate = DateTime.Now,
                Status = OrderStatus.Done,
                UserId = userId
            };

            var foodOrder = new FoodOrder()
            {
                FoodId = foodId,
                Order = order
            };

            this.data.Orders.Add(order);
            this.data.FoodOrders.Add(foodOrder);

            this.data.SaveChanges();
        }
    }
}
