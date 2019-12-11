using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static PetStore.Models.DataValidation.DataValidation;
using static PetStore.Models.DataValidation.DataValidation.Category;
using System.Text;

namespace PetStore.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(NameMaxLenght)]
        public string Name { get; set; }
        [MaxLength(DescriptionMaxLengh)]
        public string Description { get; set; }
        public ICollection<Pet> Pets { get; set; } = new HashSet<Pet>();
        public ICollection<Toy> Toys { get; set; } = new HashSet<Toy>();
        public ICollection<Food> Food { get; set; } = new HashSet<Food>();
    }
}
