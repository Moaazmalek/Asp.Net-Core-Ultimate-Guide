using CsvHelper;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services.Helpers;
using Entities;
using ServiceContracts.Enums;
using System.Globalization;
using RepositoryContracts;

namespace Services
{
    public class PersonsService : IPersonsService
    {
        private readonly IPersonsRepository _personsRepository;
        public PersonsService(IPersonsRepository personsRepository)
        {
            _personsRepository = personsRepository;
            
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

        public async Task<List<PersonResponse>> GetAllPersons()
        {

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
            List<Person> persons = searchBy switch
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
            return [..persons.Select(temp => temp.ToPersonResponse())];
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
            Person? matchingPerson = await _personsRepository.GetPersonByPersonID(personUpdateRequest.PersonID);
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
            await _personsRepository.UpdatePerson(matchingPerson);
            return matchingPerson.ToPersonResponse();
        }
        public async  Task<bool> DeletePerson(Guid? personID)
        {
            if (personID == null)
                throw new ArgumentNullException(nameof(personID));

            Person? person = await _personsRepository.GetPersonByPersonID(personID.Value);
            if (person == null) return false;
            return await _personsRepository.DeletePersonByPersonID(person.PersonID);
          

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
