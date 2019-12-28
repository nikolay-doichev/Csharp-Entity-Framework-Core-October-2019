﻿namespace PetStore
{
    using PetStore.Data;
    using PetStore.Models;
    using PetStore.Models.Enumerations;
    using PetStore.Services.Implementations;
    using System;
    using System.Linq;

    public class StartUp
    {
        public static void Main()
        {
            using (var data = new PetStoreDbContext())
            {
                for (int i = 0; i < 10; i++)
                {
                    var breed = new Breed
                    {
                        Name = "Breed" + i
                    };

                    data.Breeds.Add(breed);
                }

                data.SaveChanges();

                for (int i = 0; i < 30; i++)
                {
                    var category = new Category
                    {
                        Name = "Category" + i,
                        Description = "Category Description " + i
                    };

                    data.Categories.Add(category);
                }

                data.SaveChanges();

                for (int i = 0; i < 100; i++)
                {
                    var bredId = data
                        .Breeds
                        .OrderBy(c => Guid.NewGuid())
                        .Select(c => c.Id)
                        .FirstOrDefault();

                    var categoryId = data
                        .Categories
                        .OrderBy(c => Guid.NewGuid())
                        .Select(c => c.Id)
                        .FirstOrDefault();
                    
                    var pet = new Pet
                    {
                        DateOfBirth = DateTime.UtcNow.AddDays(-60 + i),
                        Price = 50 + i,
                        Gender = (Gender)(i % 2),
                        Description = "Some random Desciption",
                        CategoryId = categoryId,
                        BreedId = bredId
                    };

                    data.Pets.Add(pet);
                }

                data.SaveChanges();
            }
        }
    }
}
