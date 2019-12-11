using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static PetStore.Models.DataValidation.DataValidation;
using static PetStore.Models.DataValidation.DataValidation.Toy;
using System.Text;

namespace PetStore.Models
{
    public class Toy
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(NameMaxLenght)]
        public string Name { get; set; }

        [MaxLength(DescriptionMaxLengh)]
        public string Description { get; set; }
        public decimal DistributorPrie { get; set; }
        public decimal Price { get; set; }
        public int BrandId { get; set; }
        public Brand Brand { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public ICollection<ToyOrder> Orders { get; set; } = new HashSet<ToyOrder>();
    }
}
