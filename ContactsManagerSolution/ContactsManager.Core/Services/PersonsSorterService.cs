
using ContactsManager.Core.Domain.Entities;
using ContactsManager.Core.Domain.RepositoryContracts;
using ContactsManager.Core.DTO;
using ContactsManager.Core.Enums;
using ContactsManager.Core.ServiceContracts;
using CsvHelper;
using Microsoft.Extensions.Logging;
using Serilog;
using SerilogTimings;
using System.Globalization;

namespace ContactsManager.Core.Services
{
    public class PersonsSorterService : IPersonsSorterService
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsGetterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;
        public PersonsSorterService(IPersonsRepository personsRepository, ILogger<PersonsGetterService> logger,IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }
         
        public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
        {

            /*
             # Using Reflections
             if(string.IsNullOrEmpty(sortBy) return allPersons;
            PropertyInfo? property=typeof(PersonResponse).GetProperty(sortBy, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if(property == null | !property.CanRead) return allPersons;
            return sortOrder switch{

                SortOrderOptions.ASC => allPersons.OrderBy(p => proprety.GetValue(p)).ToList(),
                SortOrderOptions.DESC => allPerson.OrderByDescending(p => proprety.GetValue(p)).ToList(),
            _ => allPersons;

            };
             */
            _logger.LogInformation("GetSortedPersons of PersonsService");
            if (string.IsNullOrEmpty(sortBy)) return allPersons;
            List<PersonResponse> sortedPersons = (sortBy, sortOrder) switch
            {
                (nameof(PersonResponse.PersonName), SortOrderOptions.ASC) => [.. allPersons.OrderBy(person => person.PersonName, StringComparer.OrdinalIgnoreCase)],
                (nameof(PersonResponse.PersonName), SortOrderOptions.DESC) => [.. allPersons.OrderByDescending(person => person.PersonName, StringComparer.OrdinalIgnoreCase)],
                (nameof(PersonResponse.Email), SortOrderOptions.ASC) => [.. allPersons.OrderBy(person => person.Email, StringComparer.OrdinalIgnoreCase)],
                (nameof(PersonResponse.Email), SortOrderOptions.DESC) => [.. allPersons.OrderByDescending(person => person.Email, StringComparer.OrdinalIgnoreCase)],
                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) => [.. allPersons.OrderBy(person => person.DateOfBirth)],
                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) => [.. allPersons.OrderByDescending(person => person.DateOfBirth)],
                (nameof(PersonResponse.Age), SortOrderOptions.ASC) => [.. allPersons.OrderBy(person => person.Age)],
                (nameof(PersonResponse.Age), SortOrderOptions.DESC) => [.. allPersons.OrderByDescending(person => person.Age)],
                (nameof(PersonResponse.Gender), SortOrderOptions.ASC) => [.. allPersons.OrderBy(person => person.Gender)],
                (nameof(PersonResponse.Gender), SortOrderOptions.DESC) => [.. allPersons.OrderByDescending(person => person.Gender)],
                (nameof(PersonResponse.Country), SortOrderOptions.ASC) => [.. allPersons.OrderBy(person => person.Country)],
                (nameof(PersonResponse.Country), SortOrderOptions.DESC) => [.. allPersons.OrderByDescending(person => person.Country)],
                (nameof(PersonResponse.Address), SortOrderOptions.ASC) => [.. allPersons.OrderBy(person => person.Address)],
                (nameof(PersonResponse.Address), SortOrderOptions.DESC) => [.. allPersons.OrderByDescending(person => person.Address)],
                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC) => [.. allPersons.OrderBy(person => person.ReceiveNewsLetters)],
                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC) => [.. allPersons.OrderByDescending(person => person.ReceiveNewsLetters)],
                _ => allPersons
            };
            return sortedPersons;
            
        }
         
    }
}
