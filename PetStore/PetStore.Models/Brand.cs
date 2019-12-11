using PetStore.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static PetStore.Models.DataValidation.DataValidation;

namespace PetStore.Models
{
    public class Brand
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(NameMaxLenght)]
        public string Name { get; set; }
        public ICollection<Food> Food { get; set; } = new HashSet<Food>();
        public ICollection<Toy> Toys { get; set; } = new HashSet<Toy>();


    }
}