using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pets.Application.People
{
    public interface IPeopleService
    {
        Task<IEnumerable<PersonDto>> GetPeople();
    }
}
