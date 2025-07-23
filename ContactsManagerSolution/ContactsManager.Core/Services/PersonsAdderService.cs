using Entities;
using Microsoft.Extensions.Logging;
using RepositoryContracts;
using Serilog;
using ServiceContracts;
using ServiceContracts.DTO;
using Services.Helpers;

namespace Services
{
    public class PersonsAdderService : IPersonsAdderService
    {
        private readonly IPersonsRepository _personsRepository;

        private readonly ILogger<PersonsAdderService> _logger;
        private readonly IDiagnosticContext _diagonsticContext; // For Serilog context logging

        public PersonsAdderService(IPersonsRepository personsRepository, ILogger<PersonsAdderService> logger,
                              IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagonsticContext = diagnosticContext;
        }

        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
        {
            if (personAddRequest is null)
                throw new ArgumentNullException(nameof(personAddRequest));

            ValidationHelper.ModelValidation(personAddRequest);

            Person person = personAddRequest.ToPerson();
            person.PersonID = Guid.NewGuid();

            // _db.add it just add model object or entity to the DBSet only
            await _personsRepository.AddPerson(person);

            // adding person using stored procedure
            //_db.sp_InsertPerson(person);

            PersonResponse personResponse = person.ToPersonResponse();

            return personResponse;
        }

    }
}