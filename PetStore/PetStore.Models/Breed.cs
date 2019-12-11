namespace PetStore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using static PetStore.Models.DataValidation.DataValidation;
    using System.Text;
    public class Breed
    {
        public int Id { get; set; }

        [Required]

        [MaxLength(NameMaxLenght)]
        public string Name { get; set; }
        public ICollection<Pet> Pets { get; set; } = new HashSet<Pet>();
    }
}
