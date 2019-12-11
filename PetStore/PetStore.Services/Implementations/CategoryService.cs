using PetStore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PetStore.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly PetStoreDbContext data;
        public CategoryService(PetStoreDbContext data)
        {
            this.data = data;
        }

        public bool Exists(int categoryId)
        {
            return this.data.Categories.Any(c => c.Id == categoryId);
        }
    }
}
