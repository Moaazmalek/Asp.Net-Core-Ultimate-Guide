
using ContactsManager.Core.Domain.Entities;
using ContactsManager.Core.Domain.RepositoryContracts;
using ContactsManager.Core.DTO;
using ContactsManager.Core.Helpers;
using ContactsManager.Core.ServiceContracts;
using Microsoft.Extensions.Logging;
using Serilog;

namespace ContactsManager.Core.Services
{
    public class PersonsAdderService : IPersonsAdderService
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsGetterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;
        public PersonsAdderService(IPersonsRepository personsRepository, ILogger<PersonsGetterService> logger,IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }
        
        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequets) 
        {
            ArgumentNullException.ThrowIfNull(personAddRequets);
            //if (personAddRequets.PersonName == null) throw new ArgumentException("PersonName can't be blank");
            ValidationHelper.ModelValidation(personAddRequets);
            Person person=personAddRequets.ToPerson();
            person.PersonID = Guid.NewGuid();
            await _personsRepository.AddPerson(person);
            //using Stored Procedures
            //_db.sp_InsertPerson(person);
            PersonResponse personResponse = person.ToPersonResponse();
            return personResponse;

        }
 
    }
}
