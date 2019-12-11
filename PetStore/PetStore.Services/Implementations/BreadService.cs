using PetStore.Data;
using PetStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PetStore.Services.Implementations
{
    public class BreedService : IBreedService
    {
        private readonly PetStoreDbContext data;

        public BreedService(PetStoreDbContext data)
        {
            this.data = data;
        }
        public void Add(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Breed name cannot be null or whitespace");
            }

            var bread = new Breed
            {
                Name = name
            };

            this.data.Breeds.Add(bread);
            this.data.SaveChanges();
        }

        public bool Exists(int breedId)
        {
            return this.data.Breeds.Any(b => b.Id == breedId);
        }
    }
}
