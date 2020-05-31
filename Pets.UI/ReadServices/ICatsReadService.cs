using Pets.UI.ViewModels.CatOwners;
using System.Threading.Tasks;

namespace Pets.UI.ReadServices
{
    public interface ICatsReadService
    {
        Task<CatOwnersViewModel> GetCatOwnersViewModel();
    }
}
