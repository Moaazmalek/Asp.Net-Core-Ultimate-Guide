using CsvHelper;
using Entities.DTO;
using Entities.ServiceContracts;
using Entities.ServiceContracts.Enums;
using Entities.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
namespace Entities.Services
{
    public class PersonsService : IPersonsService
    {
        private readonly ApplicationDbContext _db;
        private readonly ICountriesService _countriesService;
        public PersonsService(ApplicationDbContext personsDbContext, ICountriesService countriesService)
        {
            _db = personsDbContext;
            _countriesService = countriesService;
            
        }
        
        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequets) 
        {
            ArgumentNullException.ThrowIfNull(personAddRequets);
            //if (personAddRequets.PersonName == null) throw new ArgumentException("PersonName can't be blank");
            ValidationHelper.ModelValidation(personAddRequets);
            Person person=personAddRequets.ToPerson();
            person.PersonID = Guid.NewGuid();
            _db.Persons.Add(person);
           await _db.SaveChangesAsync();
            //using Stored Procedures
            //_db.sp_InsertPerson(person);
            PersonResponse personResponse = person.ToPersonResponse();
            return personResponse;

        }

        public async Task<List<PersonResponse>> GetAllPersons()
        {

            var persons =await _db.Persons.Include("Country").ToListAsync();
            return  persons.Select(p => p.ToPersonResponse()).ToList();

            //using stored procedures
            //return _db.sp_GetAllPersons().Select(p => ConvertPersonIntoPersonResponse(p)).ToList();
    
        }

        public async Task<PersonResponse?> GetPersonByPersonID(Guid? personID)
        {
            if (personID == null) return null;

            Person? person= await _db.Persons.Include("Country").FirstOrDefaultAsync(p => p.PersonID == personID);
            if (person == null) return null;
            return person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
        {
            List<PersonResponse> allPersons = await GetAllPersons();
            List<PersonResponse> matchingPersons = allPersons;
            if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString)) return matchingPersons;

            switch (searchBy)
            {
                case nameof(PersonResponse.PersonName):
                    matchingPersons = [.. allPersons.Where(person =>
                    !string.IsNullOrEmpty(person.PersonName) ? person.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)];
                    break;
                case nameof(PersonResponse.Email):
                    matchingPersons = [.. allPersons.Where(person =>
                    !string.IsNullOrEmpty(person.Email) ? person.Email.Contains(searchString,StringComparison.OrdinalIgnoreCase) : true)];
                    break;
                case nameof(PersonResponse.DateOfBirth):
                    matchingPersons = [.. allPersons.Where(person =>
                    (person.DateOfBirth !=null) ? person.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString,StringComparison.OrdinalIgnoreCase): true)];
                    break;
                case nameof(PersonResponse.Gender):
                    matchingPersons = [.. allPersons.Where(person =>
                    !string.IsNullOrEmpty(person.Gender) ? person.Gender.ToString().Equals(searchString,StringComparison.OrdinalIgnoreCase): true)];
                    break;
                case nameof(PersonResponse.CountryID):
                    matchingPersons = [.. allPersons.Where(person =>
                    !string.IsNullOrEmpty(person.Country) ? person.Country.Contains(searchString): true)];
                    break;
                case nameof(PersonResponse.Address):
                    matchingPersons = [.. allPersons.Where(person =>
                    !string.IsNullOrEmpty(person.Address) ? person.Address.Contains(searchString): true)];
                    break;
                default: matchingPersons = allPersons;break;
            }
            return matchingPersons;
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

        public async  Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if (personUpdateRequest == null) throw new ArgumentNullException(nameof(PersonUpdateRequest));
            ValidationHelper.ModelValidation(personUpdateRequest);
            //get matching person object to update
            Person? matchingPerson =await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonID == personUpdateRequest.PersonID);
            if (matchingPerson == null)
            {
                throw new ArgumentException("Given person id doesn't exist");
            }

            //update all details
            matchingPerson.PersonName = personUpdateRequest.PersonName;
            matchingPerson.Email = personUpdateRequest.Email;
            matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
            matchingPerson.Gender = personUpdateRequest.Gender.ToString();
            matchingPerson.CountryID = personUpdateRequest.CountryID;
            matchingPerson.Address = personUpdateRequest.Address;
            matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;
            _db.Persons.Update(matchingPerson);
           await _db.SaveChangesAsync();
            return matchingPerson.ToPersonResponse();
        }
        public async  Task<bool> DeletePerson(Guid? personID)
        {
            if (personID == null)
                throw new ArgumentNullException(nameof(personID));

          Person? person=await  _db.Persons.FirstOrDefaultAsync(p => p.PersonID == personID);
            if (person == null) return false;
            _db.Persons.Remove(person);
            await  _db.SaveChangesAsync();
            return true;

        }
        public async Task<MemoryStream> GetPersonsCSV()
        {
            MemoryStream memoryStream=new();
            StreamWriter streamWriter = new(memoryStream);

            CsvWriter csvWriter = new(streamWriter, CultureInfo.InvariantCulture, leaveOpen: true);


            csvWriter.WriteHeader<PersonResponse>(); // writes the header row : PersonID, PersonName,..
            csvWriter.NextRecord(); // /n

            List<PersonResponse> persons =await _db.Persons
                .Include("Country").Select(p => p.ToPersonResponse()).ToListAsync();
            await csvWriter.WriteRecordsAsync(persons);

            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
