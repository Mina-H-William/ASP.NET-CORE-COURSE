using Entities;
using Exceptions;
using Microsoft.Extensions.Logging;
using RepositoryContracts;
using Serilog;
using ServiceContracts;
using ServiceContracts.DTO;
using Services.Helpers;

namespace Services
{
    public class PersonsUpdaterService : IPersonsUpdaterService
    {
        private readonly IPersonsRepository _personsRepository;

        private readonly ILogger<PersonsUpdaterService> _logger;
        private readonly IDiagnosticContext _diagonsticContext; // For Serilog context logging

        public PersonsUpdaterService(IPersonsRepository personsRepository, ILogger<PersonsUpdaterService> logger,
                              IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagonsticContext = diagnosticContext;
        }

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? person_Update)
        {
            if (person_Update is null)
                throw new ArgumentNullException(nameof(Person));

            ValidationHelper.ModelValidation(person_Update);

            Person? matchingPerson = await _personsRepository.GetPersonById(person_Update.PersonID);

            if (matchingPerson is null)
                throw new InvalidPersonIDException($"Person with ID {person_Update.PersonID} does not exist.");

            await _personsRepository.UpdatePerson(person_Update.ToPerson());

            return matchingPerson.ToPersonResponse();
        }
    }
}