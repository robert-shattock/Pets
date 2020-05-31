using System.Collections.Generic;

namespace Pets.UI.ViewModels.CatOwners
{
    public class CatsByOwnerGenderViewModel
    {
        public string OwnerGender { get; set; }
        public IEnumerable<string> CatNames { get; set; }

        public CatsByOwnerGenderViewModel()
        {
            CatNames = new List<string>();
        }
    }
}
