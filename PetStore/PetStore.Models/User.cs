namespace PetStore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using static PetStore.Models.DataValidation.DataValidation;
    using static PetStore.Models.DataValidation.DataValidation.User;
    using System.Text;
    public class User
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(NameMaxLenght)]
        public string Name { get; set; }

        [Required]
        [MaxLength(EmailMaxLenght)]
        public string Email { get; set; }

        public ICollection<Order> Orders { get; set; } = new HashSet<Order>();
    }
}
