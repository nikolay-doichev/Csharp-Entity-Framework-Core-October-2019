using System.ComponentModel.DataAnnotations;

namespace PetStore.Web.ViewsModels
{
    public class CreateCategoryInputModel
    {
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }
    }
}
