using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pets.Application.People
{
    public class PeopleService : IPeopleService
    {
        //TODO Probably just leave the interface in the application layer and the implementation in new Infrastructure layer since there is no actual business logic - just a straight retrieval of people.
        
        private readonly PeopleSettingsOptions _peopleSettingsOptions;
        private readonly IHttpClientFactory _clientFactory;

        public PeopleService(IOptions<PeopleSettingsOptions> peopleSettingsOptions, IHttpClientFactory clientFactory)
        {
            _peopleSettingsOptions = peopleSettingsOptions.Value;
            _clientFactory = clientFactory;
        }
        public async Task<IEnumerable<PersonDto>> GetPeople()
        {
            using (var client = _clientFactory.CreateClient())
            {
                client.BaseAddress = new Uri(_peopleSettingsOptions.BaseUrl);
                var responseTask = await client.GetAsync(_peopleSettingsOptions.GetPeopleUrl);

                if (responseTask.IsSuccessStatusCode)
                {
                    using (var responseStream = await responseTask.Content.ReadAsStreamAsync())
                    {
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                        };
                        var people = await JsonSerializer.DeserializeAsync<IEnumerable<PersonDto>>(responseStream, options);
                        return people;
                    }                    
                }
                else
                {
                    return null; //TODO Find out what to do here... null doesn't seem right, probably throw a not found exception or something.
                }
            }
        }
    }
}
