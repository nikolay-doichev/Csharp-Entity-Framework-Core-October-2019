namespace P03_SalesDatabase.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public class Product
    {
        [Key]
        [Required]
        public int ProductId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public double Quantity  { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public ICollection<Sale> Sales { get; set; }
    }
}
