using Entities.DTO;
using Entities.ServiceContracts;
using Entities.ServiceContracts.Enums;
using Entities.Services.Helpers;
using System.ComponentModel.DataAnnotations;
namespace Entities.Services
{
    public class PersonsService : IPersonsService
    {
        private readonly List<Person> _persons;
        private readonly ICountriesService _countriesService;
        public PersonsService()
        {
            _persons = [];
            _countriesService = new CountriesService();
        }
        private PersonResponse ConvertPersonIntoPersonResponse(Person person)
        {
            PersonResponse personResponse = person.ToPersonResponse();
            personResponse.Country = _countriesService.GetCountryByCountryID(person.CountryID)?.CountryName;
            return personResponse;
        }
        public PersonResponse? AddPerson(PersonAddRequest? personAddRequets)
        {
            ArgumentNullException.ThrowIfNull(personAddRequets);
            //if (personAddRequets.PersonName == null) throw new ArgumentException("PersonName can't be blank");
            ValidationHelper.ModelValidation(personAddRequets);
            Person person=personAddRequets.ToPerson();
            person.PersonID = Guid.NewGuid();
            _persons.Add(person);
           PersonResponse personResponse= ConvertPersonIntoPersonResponse(person);
            return personResponse;

        }

        public List<PersonResponse?> GetAllPersons()
        {
            return _persons.Select(p => p.ToPersonResponse()).ToList();
    
        }

        public PersonResponse? GetPersonByPersonID(Guid? personID)
        {
            if (personID == null) return null;

            Person? person=_persons.FirstOrDefault(p => p.PersonID == personID);
            if (person == null) return null;
            return person.ToPersonResponse();
        }

        public List<PersonResponse?> GetFilteredPersons(string searchBy, string? searchString)
        {
            List<PersonResponse> allPersons = GetAllPersons();
            List<PersonResponse> matchingPersons = allPersons;
            if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString)) return matchingPersons;

            switch (searchBy)
            {
                case nameof(Person.PersonName):
                    matchingPersons = [.. allPersons.Where(person =>
                    !string.IsNullOrEmpty(person.PersonName) ? person.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)];
                    break;
                case nameof(Person.Email):
                    matchingPersons = [.. allPersons.Where(person =>
                    !string.IsNullOrEmpty(person.Email) ? person.Email.Contains(searchString,StringComparison.OrdinalIgnoreCase) : true)];
                    break;
                case nameof(Person.DateOfBirth):
                    matchingPersons = [.. allPersons.Where(person =>
                    (person.DateOfBirth !=null) ? person.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString,StringComparison.OrdinalIgnoreCase): true)];
                    break;
                case nameof(Person.Gender):
                    matchingPersons = [.. allPersons.Where(person =>
                    !string.IsNullOrEmpty(person.Gender) ? person.Gender.ToString().Contains(searchString,StringComparison.OrdinalIgnoreCase): true)];
                    break;
                case nameof(Person.CountryID):
                    matchingPersons = [.. allPersons.Where(person =>
                    !string.IsNullOrEmpty(person.Country) ? person.Country.Contains(searchString): true)];
                    break;
                case nameof(Person.Address):
                    matchingPersons = [.. allPersons.Where(person =>
                    !string.IsNullOrEmpty(person.Address) ? person.Address.Contains(searchString): true)];
                    break;
                default: matchingPersons = allPersons;break;
            }
            return matchingPersons;
        }

        public List<PersonResponse?> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
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

        public PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if (personUpdateRequest == null) throw new ArgumentNullException(nameof(PersonUpdateRequest));
            ValidationHelper.ModelValidation(personUpdateRequest);
            //get matching person object to update
            Person? matchingPerson = _persons.FirstOrDefault(temp => temp.PersonID == personUpdateRequest.PersonID);
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

            return matchingPerson.ToPersonResponse();
        }
        public bool DeletePerson(Guid? personID)
        {
            if (personID == null)
                throw new ArgumentNullException(nameof(personID));

          Person? person=  _persons.FirstOrDefault(p => p.PersonID == personID);
            if (person == null) return false;
            _persons.Remove(person);
            return true;

        }
    }
}
