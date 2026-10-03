using Entities;
using Entities.DTO;
using Entities.ServiceContracts;
using Entities.ServiceContracts.Enums;
using Entities.Services;
using Xunit.Abstractions;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personService;
        private readonly ICountriesService _countryService;
        private readonly ITestOutputHelper _testOutputHelper;
        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _personService = new PersonsService();
            _countryService = new CountriesService();
            _testOutputHelper = testOutputHelper;
        }
        #region AddPerson
        //When we supply null value as PersonAddRequest, it should throw ArgumentNullException
        [Fact]
        public void AddPerson_NullPerson()
        {
            //Arrange
            PersonAddRequest? personAddRequest = null;
            //Act 
            Assert.Throws<ArgumentNullException>(() =>
            {
                _personService.AddPerson(personAddRequest);

            });
        }
        //When we supply a PersonName as null value, it should throw ArgumentException
        [Fact]
        public void AddPerson_PersonNameIsNull()
        {
            //Arrange
            PersonAddRequest? personAddRequest = new()
            {
                PersonName = null
            };
            //Act 
            Assert.Throws<ArgumentException>(() =>
            {
                _personService.AddPerson(personAddRequest);

            });
        }
        //When we supply proper PersonDetails , it should insert the person into the persons list
        [Fact]
         public void AddPerson_ProperPersonDetails()
        {
            //Arrange
            PersonAddRequest? personAddRequest = new()
            {
                PersonName = "John Doe",
                Email = "john.doe@example.com",
                DateOfBirth = new DateTime(1990, 5, 15),
                Gender = GenderOptions.Male,
                Address = "123 Main Street, Springfield, IL 62701",
                CountryID=Guid.NewGuid(),
                ReceiveNewsLetters=false
            };
            //Act 
            
             PersonResponse person_response_from_add= _personService.AddPerson(personAddRequest);
            List<PersonResponse> persons_list = _personService.GetAllPersons();
            //Assert
            Assert.True(person_response_from_add.PersonID != Guid.Empty);
            Assert.Contains(person_response_from_add, persons_list);

            
        }
        #endregion
        #region GetPersonByPersonID
        //If we supply null as personID, it should return null as PersonResponse
        [Fact]
        public void GetPersonByPersonID_NullPersonID()
        {
            //Arrnge 
            Guid? personID = null;
            //Act
          PersonResponse? person_response_from_get=  _personService.GetPersonByPersonID(personID);
            Assert.Null(person_response_from_get);
        }
        //if we supply a valid person id, it should return the valid person details as PersonResponse obj
        [Fact]
        public void GetPersonByPersonID_WithPersonID()
        {
            //Arrange
            CountryAddRequest country_request = new()
            {
                CountryName = "Canada"
            };
            CountryResponse country_response= _countryService.AddCountry(country_request);

            //Act
            PersonAddRequest person_request = new()
            {
                PersonName = "John Doe",
                Email = "john.doe@example.com",
                DateOfBirth = new DateTime(1990, 5, 15),
                Gender = GenderOptions.Male,
                Address = "123 Main Street, Springfield, IL 62701",
                ReceiveNewsLetters = false,
                CountryID = country_response.CountryID
            };
            PersonResponse person_response_from_add= _personService.AddPerson(person_request);
            PersonResponse? person_response_from_get= _personService.GetPersonByPersonID(person_response_from_add.PersonID);
            //Assert
            Assert.Equal(person_response_from_add, person_response_from_get);
        }
        #endregion
        #region GetAllPersons 

        // the GetAllPersons() should return an empty list by default 
        [Fact]
        public void GetAllPersons_EmptyList()
        {
            //Act 
            List<PersonResponse?> persons_response_from_get=_personService.GetAllPersons();
            //Assert
            Assert.Empty(persons_response_from_get);
        }
        // First, we will add few persons; and then when we call GetAllPersons(), it should return the same persons were added
        [Fact]
        public void GetAllPersons_AddFewPersons()
        {
            //Arrange
            CountryAddRequest country_request_1 = new()
            {
                CountryName = "Jordan"
            };
            CountryAddRequest country_request_2 = new()
            {
                CountryName = "Syria"
            };
            CountryResponse country_response_1 = _countryService.AddCountry(country_request_1);
            CountryResponse country_response_2 = _countryService.AddCountry(country_request_2);
            PersonAddRequest person_request_1 = new()
            {
                PersonName = "Muath Malek",
                Email = "muathmalek@example.com",
                DateOfBirth = new DateTime(2003, 12, 16),
                Gender = GenderOptions.Male,
                Address = "123 Main Street, Springfield, IL 62701",
                ReceiveNewsLetters = false,
                CountryID = country_response_2.CountryID
            };
            PersonAddRequest person_request_2 = new()
            {
                PersonName = "Hanaa Abd-Alfatah",
                Email = "hanaa@example.com",
                DateOfBirth = new DateTime(1982, 10, 12),
                Gender = GenderOptions.Female,
                Address = "123 Main Street, Springfield, IL 62701",
                ReceiveNewsLetters = false,
                CountryID = country_response_1.CountryID
            };
            PersonAddRequest person_request_3 = new()
            {
                PersonName = "Hamza Zamil",
                Email = "hamzazamil@example.com",
                DateOfBirth = new DateTime(1982, 10, 12),
                Gender = GenderOptions.Male,
                Address = "123 Main Street, Springfield, IL 62701",
                ReceiveNewsLetters = false,
                CountryID = country_response_1.CountryID
            };
            
            List<PersonAddRequest> person_requests = [person_request_1, person_request_2, person_request_3];
            List<PersonResponse> person_response_from_add = [];
            foreach(PersonAddRequest person in person_requests)
            {
               PersonResponse person_response= _personService.AddPerson(person);
                person_response_from_add.Add(person_response);
            }
            //print person_response_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach(PersonResponse person_response in person_response_from_add)
            {
                _testOutputHelper.WriteLine(person_response.ToString());
            }
           
            //Act 
           List<PersonResponse> persons_list_from_get= _personService.GetAllPersons();
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse  person_response_from_list in persons_list_from_get) 
            {
                _testOutputHelper.WriteLine(person_response_from_list.ToString()); 
            }
                //Assert
                foreach (PersonResponse person_response in person_response_from_add)
            {
                Assert.Contains(person_response, persons_list_from_get);
            }
            
        }
        #endregion
        #region GetFilteredPersons
        //If the search test is empty and search by is "PersonName", it should return all persons 
        [Fact]
        public void GetFilteredPersons_EmptySearchText()
        {
            //Arrange
            CountryAddRequest country_request_1 = new()
            {
                CountryName = "Jordan"
            };
            CountryAddRequest country_request_2 = new()
            {
                CountryName = "Syria"
            };
            CountryResponse country_response_1 = _countryService.AddCountry(country_request_1);
            CountryResponse country_response_2 = _countryService.AddCountry(country_request_2);
            PersonAddRequest person_request_1 = new()
            {
                PersonName = "Muath Malek",
                Email = "muathmalek@example.com",
                DateOfBirth = new DateTime(2003, 12, 16),
                Gender = GenderOptions.Male,
                Address = "123 Main Street, Springfield, IL 62701",
                ReceiveNewsLetters = false,
                CountryID = country_response_2.CountryID
            };
            PersonAddRequest person_request_2 = new()
            {
                PersonName = "Hanaa Abd-Alfatah",
                Email = "hanaa@example.com",
                DateOfBirth = new DateTime(1982, 10, 12),
                Gender = GenderOptions.Female,
                Address = "123 Main Street, Springfield, IL 62701",
                ReceiveNewsLetters = false,
                CountryID = country_response_1.CountryID
            };
            PersonAddRequest person_request_3 = new()
            {
                PersonName = "Hamza Zamil",
                Email = "hamzazamil@example.com",
                DateOfBirth = new DateTime(1982, 10, 12),
                Gender = GenderOptions.Male,
                Address = "123 Main Street, Springfield, IL 62701",
                ReceiveNewsLetters = false,
                CountryID = country_response_1.CountryID
            };

            List<PersonAddRequest> person_requests = [person_request_1, person_request_2, person_request_3];
            List<PersonResponse> person_response_from_add = [];
            foreach (PersonAddRequest person in person_requests)
            {
                PersonResponse person_response = _personService.AddPerson(person);
                person_response_from_add.Add(person_response);
            }
            //print person_response_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response in person_response_from_add)
            {
                _testOutputHelper.WriteLine(person_response.ToString());
            }

            //Act 
            List<PersonResponse> persons_list_from_search = _personService.GetFilteredPersons(searchBy:nameof(Person.PersonName), searchString: "");
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_list in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_list.ToString());
            }
            //Assert
            foreach (PersonResponse person_response in person_response_from_add)
            {
                Assert.Contains(person_response, persons_list_from_search);
            }
        }
        //First we will add few persons; and then we will search based on person name wit some search string. It should
        //return the matching persons

        [Fact]
        public void GetFilteredPersons_SearchByPersonName()
        {
            //Arrange
            CountryAddRequest country_request_1 = new()
            {
                CountryName = "Jordan"
            };
            CountryAddRequest country_request_2 = new()
            {
                CountryName = "Syria"
            };
            CountryResponse country_response_1 = _countryService.AddCountry(country_request_1);
            CountryResponse country_response_2 = _countryService.AddCountry(country_request_2);
            PersonAddRequest person_request_1 = new()
            {
                PersonName = "Muath",
                Email = "muathmalek@example.com",
                DateOfBirth = new DateTime(2003, 12, 16),
                Gender = GenderOptions.Male,
                Address = "123 Main Street, Springfield, IL 62701",
                ReceiveNewsLetters = false,
                CountryID = country_response_2.CountryID
            };
            PersonAddRequest person_request_2 = new()
            {
                PersonName = "Hanaa",
                Email = "hanaa@example.com",
                DateOfBirth = new DateTime(1982, 10, 12),
                Gender = GenderOptions.Female,
                Address = "123 Main Street, Springfield, IL 62701",
                ReceiveNewsLetters = false,
                CountryID = country_response_1.CountryID
            };
            PersonAddRequest person_request_3 = new()
            {
                PersonName = "Hamza",
                Email = "hamzazamil@example.com",
                DateOfBirth = new DateTime(1982, 10, 12),
                Gender = GenderOptions.Male,
                Address = "123 Main Street, Springfield, IL 62701",
                ReceiveNewsLetters = false,
                CountryID = country_response_1.CountryID
            };

            List<PersonAddRequest> person_requests = [person_request_1, person_request_2, person_request_3];
            List<PersonResponse> person_response_from_add = [];
            foreach (PersonAddRequest person in person_requests)
            {
                PersonResponse person_response = _personService.AddPerson(person);
                person_response_from_add.Add(person_response);
            }
            //print person_response_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response in person_response_from_add)
            {
                _testOutputHelper.WriteLine(person_response.ToString());
            }

            //Act 
            List<PersonResponse> persons_list_from_search = _personService.GetFilteredPersons(searchBy: nameof(Person.PersonName), searchString: "mu");
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_list in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_list.ToString());
            }
            //Assert
            foreach (PersonResponse person_response in person_response_from_add)
            {
                if (person_response.PersonName != null)
                {
                    if (person_response.PersonName.Contains("mu", StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.Contains(person_response, persons_list_from_search);

                    }
                }
            }
        }

        #endregion
        #region GetSortedPersons
        // When we sort based on PersonName in DESC, it should return persons list in descinding on PersonName
        [Fact]
        public void GetSortedPersons_DESC()
        {

            //Arrange
            CountryAddRequest country_request_1 = new()
            {
                CountryName = "Jordan"
            };
            CountryAddRequest country_request_2 = new()
            {
                CountryName = "Syria"
            };
            CountryResponse country_response_1 = _countryService.AddCountry(country_request_1);
            CountryResponse country_response_2 = _countryService.AddCountry(country_request_2);
            PersonAddRequest person_request_1 = new()
            {
                PersonName = "Muath",
                Email = "muathmalek@example.com",
                DateOfBirth = new DateTime(2003, 12, 16),
                Gender = GenderOptions.Male,
                Address = "123 Main Street, Springfield, IL 62701",
                ReceiveNewsLetters = false,
                CountryID = country_response_2.CountryID
            };
            PersonAddRequest person_request_2 = new()
            {
                PersonName = "Hanaa",
                Email = "hanaa@example.com",
                DateOfBirth = new DateTime(1982, 10, 12),
                Gender = GenderOptions.Female,
                Address = "123 Main Street, Springfield, IL 62701",
                ReceiveNewsLetters = false,
                CountryID = country_response_1.CountryID
            };
            PersonAddRequest person_request_3 = new()
            {
                PersonName = "Hamza",
                Email = "hamzazamil@example.com",
                DateOfBirth = new DateTime(1982, 10, 12),
                Gender = GenderOptions.Male,
                Address = "123 Main Street, Springfield, IL 62701",
                ReceiveNewsLetters = false,
                CountryID = country_response_1.CountryID
            };

            List<PersonAddRequest> person_requests = [person_request_1, person_request_2, person_request_3];
            List<PersonResponse> person_response_from_add = [];
            foreach (PersonAddRequest person in person_requests)
            {
                PersonResponse person_response = _personService.AddPerson(person);
                person_response_from_add.Add(person_response);
            }
            //print person_response_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response in person_response_from_add)
            {
                _testOutputHelper.WriteLine(person_response.ToString());
            }
            List<PersonResponse> allPersons = _personService.GetAllPersons();
            //Act 
            List<PersonResponse> persons_list_from_sort= _personService.GetSortedPersons(allPersons,nameof(Person.PersonName),SortOrderOptions.DESC);
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_list in persons_list_from_sort)
            {
                _testOutputHelper.WriteLine(person_response_from_list.ToString());
            }
            person_response_from_add = person_response_from_add.OrderByDescending(person => person.PersonName).ToList();

            //Assert
             for(int i = 0; i < person_response_from_add.Count; i++)
            {
                Assert.Equal(person_response_from_add[i], persons_list_from_sort[i]);
            }
        }
        [Fact]
        public void GetSortedPersons_ASC()
        {

            //Arrange
            CountryAddRequest country_request_1 = new()
            {
                CountryName = "Jordan"
            };
            CountryAddRequest country_request_2 = new()
            {
                CountryName = "Syria"
            };
            CountryResponse country_response_1 = _countryService.AddCountry(country_request_1);
            CountryResponse country_response_2 = _countryService.AddCountry(country_request_2);
            PersonAddRequest person_request_1 = new()
            {
                PersonName = "Muath",
                Email = "muathmalek@example.com",
                DateOfBirth = new DateTime(2003, 12, 16),
                Gender = GenderOptions.Male,
                Address = "123 Main Street, Springfield, IL 62701",
                ReceiveNewsLetters = false,
                CountryID = country_response_2.CountryID
            };
            PersonAddRequest person_request_2 = new()
            {
                PersonName = "Hanaa",
                Email = "hanaa@example.com",
                DateOfBirth = new DateTime(1982, 10, 12),
                Gender = GenderOptions.Female,
                Address = "123 Main Street, Springfield, IL 62701",
                ReceiveNewsLetters = false,
                CountryID = country_response_1.CountryID
            };
            PersonAddRequest person_request_3 = new()
            {
                PersonName = "Hamza",
                Email = "hamzazamil@example.com",
                DateOfBirth = new DateTime(1982, 10, 12),
                Gender = GenderOptions.Male,
                Address = "123 Main Street, Springfield, IL 62701",
                ReceiveNewsLetters = false,
                CountryID = country_response_1.CountryID
            };

            List<PersonAddRequest> person_requests = [person_request_1, person_request_2, person_request_3];
            List<PersonResponse> person_response_from_add = [];
            foreach (PersonAddRequest person in person_requests)
            {
                PersonResponse person_response = _personService.AddPerson(person);
                person_response_from_add.Add(person_response);
            }
            //print person_response_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response in person_response_from_add)
            {
                _testOutputHelper.WriteLine(person_response.ToString());
            }
            List<PersonResponse> allPersons = _personService.GetAllPersons();
            //Act 
            List<PersonResponse> persons_list_from_sort = _personService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.ASC);
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_list in persons_list_from_sort)
            {
                _testOutputHelper.WriteLine(person_response_from_list.ToString());
            }
            person_response_from_add = person_response_from_add.OrderBy(person => person.PersonName).ToList();

            //Assert
            for (int i = 0; i < person_response_from_add.Count; i++)
            {
                Assert.Equal(person_response_from_add[i], persons_list_from_sort[i]);
            }
        }
        #endregion
        #region UpdatePerson

        // When we supply null as PersonUpdateRequest, it should throw ArgumentNullException
        [Fact]
        public void UpdatePerson_NullPerson()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = null;
            Assert.Throws<ArgumentNullException>(() =>
            {
                //Act 
                _personService.UpdatePerson(personUpdateRequest);
            });
        }

        // When we supply invalid personID, it should throw ArgumentException
        [Fact]
        public void UpdatePerson_InvalidPersonID()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = new()
            {
                PersonID = Guid.NewGuid()
            };
            //Assert 
            Assert.Throws<ArgumentException>(() =>
            {
                //Act 
                _personService.UpdatePerson(personUpdateRequest);
            });
        }
        // When the personName is null, it should throw ArgumentException
        [Fact]
        public void UpdatePerson_NullPersonName()
        {
            //Arrange
            CountryAddRequest country_add_request = new()
            {
                CountryName = "UK"
            };
            CountryResponse country_response_from_add=  _countryService.AddCountry(country_add_request);

            PersonAddRequest person_add_request = new()
            {
                PersonName = "John",
                CountryID = country_response_from_add.CountryID,
                Email="John@example.com",
                Address="Address...",
                Gender=GenderOptions.Male
            };
            PersonResponse person_response_from_add=_personService.AddPerson(person_add_request);
            PersonUpdateRequest? personUpdateRequest = person_response_from_add.ToPersonUpdateRequest();
            personUpdateRequest.PersonName = null;
            //Assert 
            Assert.Throws<ArgumentException>(() =>
            {
                //Act 
                _personService.UpdatePerson(personUpdateRequest);
            });
        }
        // First, add a new person and try to update the same
        [Fact]
        public void UpdatePerson_PersonFullDetails()
        {
            //Arrange
            CountryAddRequest country_add_request = new()
            {
                CountryName = "UK"
            };
            CountryResponse country_response_from_add=  _countryService.AddCountry(country_add_request);

            PersonAddRequest person_add_request = new()
            {
                PersonName = "John",
                CountryID = country_response_from_add.CountryID,
                Email="JohnDoe@example.com",
                Address="Amman / Jordan",
                DateOfBirth=DateTime.Parse("2000-01-01"),
                ReceiveNewsLetters=false,
                Gender=GenderOptions.Male
            };
            PersonResponse person_response_from_add=_personService.AddPerson(person_add_request);
            PersonUpdateRequest? personUpdateRequest = person_response_from_add.ToPersonUpdateRequest();
            personUpdateRequest.PersonName = "William";
            personUpdateRequest.Email = "william@example.com";

            //Act 
             PersonResponse person_response_from_update= _personService.UpdatePerson(personUpdateRequest);
            PersonResponse person_response_from_get= _personService.GetPersonByPersonID(person_response_from_update.PersonID);

            //Assert
            Assert.Equal(person_response_from_get, person_response_from_update);
            
        }
        #endregion
        #region DeletePerson
        //If you supply a valid PersonID, it should return true;
        [Fact]
        public void DeletePerson_ValidPersonID()
        {
            //Arrange
            CountryAddRequest country_add_request = new()
            {
                CountryName = "USA"
            };
            CountryResponse country_response_from_add = _countryService.AddCountry(country_add_request);
            PersonAddRequest person_add_request = new()
            {
                PersonName = "John",
                Email = "John@gmail.com",
                Address = "Address...",
                CountryID = country_response_from_add.CountryID,
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                ReceiveNewsLetters=false
            };
            PersonResponse person_response_from_add = _personService.AddPerson(person_add_request);

            //Act 
            bool isDeleted=_personService.DeletePerson(person_response_from_add.PersonID);
            //Assert
            Assert.True(isDeleted);

        }
        //If you supply an invalid PersonID, it should return false;
        [Fact]
        public void DeletePerson_InvalidPersonID()
        {
         

            //Act 
            bool isDeleted=_personService.DeletePerson(Guid.NewGuid());
            //Assert
            Assert.False(isDeleted);

        }
        #endregion
    }
}
