namespace PetStore.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using PetStore.Services.Models.Pet;
    using Services;
    using Models.Pet;
    using System.Collections.Generic;

    public class PetsController : Controller
    {
        private readonly IPetService pets;

        public PetsController(IPetService pets)
        {
            this.pets = pets;
        }
        public IActionResult All(int page = 1) 
        {
            var allPets = this.pets.All(page);
            var totalPets = this.pets.Total();

            var model = new AllPetsViewModel
            {
                Pets = allPets,
                CurrentPage = page,
                Total = totalPets
            };

            return View(model);
        }

        public IActionResult Delete(int id)
        {
            var pet = this.pets.Details(id);

            if (pet == null)
            {
                return NotFound();
            }

            return View(pet);
        }

        public IActionResult ConfirmDelete(int id)
        {
            var delete = this.pets.Delete(id);

            if (!delete)
            {
                return BadRequest();
            }

            return RedirectToAction(nameof(All));
        }
    }
}
