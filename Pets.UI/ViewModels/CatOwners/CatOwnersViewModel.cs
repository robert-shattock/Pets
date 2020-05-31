using System.Collections.Generic;

namespace Pets.UI.ViewModels.CatOwners
{
    public class CatOwnersViewModel
    {
        public IEnumerable<CatsByOwnerGenderViewModel> CatsByGender { get; set; }

        public CatOwnersViewModel()
        {
            CatsByGender = new List<CatsByOwnerGenderViewModel>();
        }
    }
}
