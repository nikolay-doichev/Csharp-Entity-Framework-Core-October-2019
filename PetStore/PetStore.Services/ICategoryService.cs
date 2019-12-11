using System;
using System.Collections.Generic;
using System.Text;

namespace PetStore.Services
{
    public interface ICategoryService
    {
        bool Exists(int categoryId);
    }
}
