using CsvHelper;
using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using System.Globalization;
using RepositoryContracts;
using Microsoft.Extensions.Logging;
using Serilog;
using SerilogTimings;
namespace Services
{
    public class PersonsGetterService : IPersonsGetterService
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsGetterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;
        public PersonsGetterService(IPersonsRepository personsRepository, ILogger<PersonsGetterService> logger,IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }
        
   
        public async Task<List<PersonResponse>> GetAllPersons()
        {
            _logger.LogInformation("GetAllPersons of PersonsService");
            var persons = await _personsRepository.GetAllPersons();
            return [.. persons.Select(p => p.ToPersonResponse())];

            //using stored procedures
            //return _db.sp_GetAllPersons().Select(p => ConvertPersonIntoPersonResponse(p)).ToList();
    
        }

        public async Task<PersonResponse?> GetPersonByPersonID(Guid? personID)
        {
            if (personID == null) return null;

            Person? person = await _personsRepository.GetPersonByPersonID(personID.Value);
            if (person == null) return null;
            return person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
        {
            _logger.LogInformation("GetFilteredPersons of PersonsService");

            List<Person> persons;
            using (Operation.Time("Time for Filtered Persons from Database")) { 
             persons = searchBy switch
            {
                nameof(PersonResponse.PersonName) => await _personsRepository.
                GetFilteredPersons(temp => temp.PersonName.Contains(searchString)),
                nameof(PersonResponse.Email) => await _personsRepository
                .GetFilteredPersons(temp => temp.Email.Contains(searchString)),
                nameof(PersonResponse.DateOfBirth) => await _personsRepository.
                GetFilteredPersons(temp => temp.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString)),
                nameof(PersonResponse.Gender) => await _personsRepository.
                GetFilteredPersons(temp => temp.Gender.Contains(searchString)),
                nameof(PersonResponse.CountryID) => await _personsRepository.
                GetFilteredPersons(temp => temp.Country.CountryName.Contains(searchString)),
                nameof(PersonResponse.Address) => await _personsRepository.
                GetFilteredPersons(temp => temp.Address.Contains(searchString)),
                _ => await _personsRepository.GetAllPersons()
            };
            }
            _diagnosticContext.Set("Persons", persons);
            return [..persons.Select(temp => temp.ToPersonResponse())];
        }
 
        public async Task<MemoryStream> GetPersonsCSV()
        {
            MemoryStream memoryStream=new();
            StreamWriter streamWriter = new(memoryStream);

            CsvWriter csvWriter = new(streamWriter, CultureInfo.InvariantCulture, leaveOpen: true);


            csvWriter.WriteHeader<PersonResponse>(); // writes the header row : PersonID, PersonName,..
            csvWriter.NextRecord(); // /n

            List<PersonResponse> persons = [.. (await _personsRepository.GetAllPersons()).Select(temp => temp.ToPersonResponse())];

                
            await csvWriter.WriteRecordsAsync(persons);

            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
