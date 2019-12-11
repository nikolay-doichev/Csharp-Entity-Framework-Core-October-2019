using PetStore.Models.Enumerations;
using System;
using System.Collections.Generic;
using static PetStore.Models.DataValidation.DataValidation.Pet;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace PetStore.Models
{
    public class Pet
    {
        public int Id { get; set; }
        public Gender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public decimal Price { get; set; }

        [MaxLength(DescriptionMaxLengh)]
        public string Description { get; set; }
        public int BreedId { get; set; }
        public Breed Breed { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int? OrderId { get; set; }
        public Order Order { get; set; }
    }
}
