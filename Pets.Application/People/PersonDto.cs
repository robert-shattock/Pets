using System.Collections.Generic;

namespace Pets.Application.People
{
    public class PersonDto
    {
        public string Name { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public IEnumerable<PetDto> Pets {get; set;}
    }
}
