using PetStore.Data;
using PetStore.Models;
using PetStore.Services.Models.Categorie;
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

        public IEnumerable<AllCategoriesServiceModel> All()
        {
            return this.data
                .Categories
                .Select(c => new AllCategoriesServiceModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                })
                .ToArray();
        }

        public void Create(CreateCategoryServiceModel model)
        {
            var category = new Category()
            {
                Name = model.Name,
                Description = model.Description
            };

            this.data.Categories.Add(category);
            this.data.SaveChanges();
        }

        public bool Exists(int categoryId)
        {
            return this.data.Categories.Any(c => c.Id == categoryId);
        }

        public DetailsCategoryServiceModel GetById(int id)
        {
            var category = this.data
                .Categories
                .Find(id);

            var dscm = new DetailsCategoryServiceModel
            {
                Name = category?.Name,
                Description = category?.Description
            };

            return dscm;
        }
    }
}
