
using ContactsManager.Core.Domain.Entities;
using ContactsManager.Core.Domain.RepositoryContracts; 
using ContactsManager.Core.ServiceContracts; 
using Microsoft.Extensions.Logging;
using Serilog; 

namespace ContactsManager.Core.Services
{
    public class PersonsDeleterService : IPersonsDeleterService
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsGetterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;
        public PersonsDeleterService(IPersonsRepository personsRepository, ILogger<PersonsGetterService> logger,IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }
          
        public async  Task<bool> DeletePerson(Guid? personID)
        {
            if (personID == null)
                throw new ArgumentNullException(nameof(personID));

            Person? person = await _personsRepository.GetPersonByPersonID(personID.Value);
            if (person == null) return false;
            return await _personsRepository.DeletePersonByPersonID(person.PersonID);
          

        } 
    }
}
