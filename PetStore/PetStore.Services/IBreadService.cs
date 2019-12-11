using System;
using System.Collections.Generic;
using System.Text;

namespace PetStore.Services
{
    public interface IBreedService
    {
        void Add(string name);

        bool Exists(int breedId);
    }
}
