using Pets.Application.People;
using Pets.UI.ViewModels.CatOwners;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Pets.UI.ReadServices
{
    public class CatsReadService : ICatsReadService
    {
        private readonly IPeopleService _peopleService;
        private const string IdentifierForPetTypeOfCat = "Cat"; //TODO Not sure if this is the best place/name... 
                                                                //how do I use this in tests... do I need an enum or constant class etc?
                                                                //Should the PeopleService.GetPeople return an enum or something
                                                                //for the pet type rather than relying on a magic string here
                                                                //that might get out of date e.g. what happens if Cat changes
                                                                //to Feline down the track in the People.json?

        public CatsReadService(IPeopleService peopleService)
        {
            _peopleService = peopleService;
        }
        
        public async Task<CatOwnersViewModel> GetCatOwnersViewModel()
        {
            var people = await _peopleService.GetPeople();

            //Each person has a collection of pets... we want to find all the pets and their associated owners' genders,
            //filter them to just cats, and then group them by owner gender, with everything sorted alphabetically.

            //NOTE: Lambda equivalent would be to use SelectMany
            var flatListOfAllCatsAndOwnerGender = (from owner in people
                                                   where owner.Pets != null
                                                   from pet in owner.Pets
                                                   where pet.Type == IdentifierForPetTypeOfCat
                                                   select new { owner.Gender, CatName = pet.Name });

            var catsByGenderViewModels = (from cat in flatListOfAllCatsAndOwnerGender
                                          group cat by cat.Gender into catsGroupedByOwnerGender
                                          orderby catsGroupedByOwnerGender.Key
                                          select new CatsByOwnerGenderViewModel() { 
                                          OwnerGender = catsGroupedByOwnerGender.Key, 
                                          CatNames = catsGroupedByOwnerGender.Select(c => c.CatName).OrderBy(catName => catName) });

            var catOwnersViewModel = new CatOwnersViewModel() { CatsByGender = catsByGenderViewModels };

            return catOwnersViewModel;
        }

    }

}
