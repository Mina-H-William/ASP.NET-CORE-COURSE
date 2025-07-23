using Microsoft.Extensions.Logging;
using RepositoryContracts;
using Serilog;
using ServiceContracts;

namespace Services
{
    public class PersonsDeleterService : IPersonsDeleterService
    {
        private readonly IPersonsRepository _personsRepository;

        private readonly ILogger<PersonsDeleterService> _logger;
        private readonly IDiagnosticContext _diagonsticContext; // For Serilog context logging

        public PersonsDeleterService(IPersonsRepository personsRepository, ILogger<PersonsDeleterService> logger,
                              IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagonsticContext = diagnosticContext;
        }

        public async Task<bool> DeletePerson(Guid? personID)
        {
            if (personID is null)
                throw new ArgumentNullException(nameof(personID));

            return await _personsRepository.DeletePersonByPersonID(personID.Value);
        }
    }
}