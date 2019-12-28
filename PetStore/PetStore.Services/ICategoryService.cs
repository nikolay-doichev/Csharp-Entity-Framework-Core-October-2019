using PetStore.Services.Models.Categorie;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetStore.Services
{
    public interface ICategoryService
    {
        DetailsCategoryServiceModel GetById(int id);

        void Create(CreateCategoryServiceModel model);
        bool Exists(int categoryId);

        IEnumerable<AllCategoriesServiceModel> All();
    }
}
