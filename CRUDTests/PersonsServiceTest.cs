using Entities;
using Entities.DTO;
using Entities.ServiceContracts;
using Entities.ServiceContracts.Enums;
using Entities.Services;
using Xunit.Abstractions;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCoreMock;
using AutoFixture;
using FluentAssertions;
namespace CRUDTests
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personService;
        private readonly ICountriesService _countryService;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;
        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture();
            var countriesInitialData = new List<Country>();
            var personsInitialData = new List<Person>();
            DbContextMock<ApplicationDbContext> dbContextMock = new(new DbContextOptionsBuilder<ApplicationDbContext>().Options);
            ApplicationDbContext dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
            dbContextMock.CreateDbSetMock(temp => temp.Persons, personsInitialData);
            _countryService = new CountriesService(dbContext);
             _testOutputHelper = testOutputHelper;
            _personService = new PersonsService(dbContext,_countryService);
         }
        #region AddPerson
        //When we supply null value as PersonAddRequest, it should throw ArgumentNullException
        [Fact]
        public async Task AddPerson_NullPerson()
        {
            //Arrange
            PersonAddRequest? personAddRequest = null;
            //Act 

            Func<Task> action = (async () =>
            {
                await _personService.AddPerson(personAddRequest);
            });
            //Assertion
             await action.Should().ThrowAsync<ArgumentNullException>();
           
        }
        //When we supply a PersonName as null value, it should throw ArgumentException
        [Fact]
        public async Task AddPerson_PersonNameIsNull()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, null as string)
                .Create();

            Func<Task> action = async () =>
            {
                //Act
                await _personService.AddPerson(personAddRequest);
            };

            //Assertion
            await action.Should().ThrowAsync<ArgumentException>();
            //await Assert.ThrowsAsync<ArgumentException>(async () =>
            //{
            //    await _personService.AddPerson(personAddRequest);

            //});
        }
        //When we supply proper PersonDetails , it should insert the person into the persons list
        [Fact]
         public async Task AddPerson_ProperPersonDetails()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture
                .Build<PersonAddRequest>()
                .With(temp => temp.Email,"someone@gmail.com")
                .Create(); 
            //Act 
            
             PersonResponse person_response_from_add= await _personService.AddPerson(personAddRequest);
            List<PersonResponse> persons_list = await _personService.GetAllPersons();
            //Assert
            person_response_from_add.Should().NotBe(Guid.Empty);
            persons_list.Should().Contain(person_response_from_add);

            //Assert.True(person_response_from_add.PersonID != Guid.Empty);
            //Assert.Contains(person_response_from_add, persons_list);

            
        }
        #endregion
        #region GetPersonByPersonID
        //If we supply null as personID, it should return null as PersonResponse
        [Fact]
        public async Task GetPersonByPersonID_NullPersonID()
        {
            //Arrnge 
            Guid? personID = null;
            //Act
          PersonResponse? person_response_from_get= await _personService.GetPersonByPersonID(personID);
            person_response_from_get.Should().BeNull();
            //Assert.Null(person_response_from_get);
        }
        //if we supply a valid person id, it should return the valid person details as PersonResponse obj
        [Fact]
        public async Task GetPersonByPersonID_WithPersonID()
        {
            //Arrange
            CountryAddRequest country_request = _fixture.Create<CountryAddRequest>();
            CountryResponse country_response= await _countryService.AddCountry(country_request);

            //Act
            PersonAddRequest person_request = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.CountryID, country_response.CountryID)
                .With(temp => temp.Email, "email@sample.com")
                .Create();
            PersonResponse person_response_from_add= await _personService.AddPerson(person_request);
            PersonResponse? person_response_from_get= await _personService.GetPersonByPersonID(person_response_from_add.PersonID);
            //Assert
            person_response_from_get.Should().Be(person_response_from_add);
            //Assert.Equal(person_response_from_add, person_response_from_get);
        }
        #endregion
        #region GetAllPersons 

        // the GetAllPersons() should return an empty list by default 
        [Fact]
        public async Task GetAllPersons_EmptyList()
        {
            //Act 
            List<PersonResponse> persons_response_from_get= await _personService.GetAllPersons();
            //Assert
            persons_response_from_get.Should().BeEmpty();
            //Assert.Empty(persons_response_from_get);
        }
        // First, we will add few persons; and then when we call GetAllPersons(), it should return the same persons were added
        [Fact]
        public async Task GetAllPersons_AddFewPersons()
        {
            //Arrange
            CountryAddRequest country_request_1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest country_request_2 = _fixture.Create<CountryAddRequest>();
            CountryResponse country_response_1 = await _countryService.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countryService.AddCountry(country_request_2);
            PersonAddRequest person_request_1 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "someone_3@example.com")
                .With(temp => temp.CountryID, country_response_1.CountryID)
                .Create();
            PersonAddRequest person_request_2 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "someone_4@example.com")
                .With(temp => temp.CountryID, country_response_2.CountryID)
                .Create();
            PersonAddRequest person_request_3 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "someone_35@example.com")
                .With(temp => temp.CountryID, country_response_1.CountryID)
                .Create();
            List<PersonAddRequest> person_requests = [person_request_1, person_request_2, person_request_3];
            List<PersonResponse> person_responses_from_add = [];

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = await _personService.AddPerson(person_request);
                person_responses_from_add.Add(person_response);
            }

            //Act
            List<PersonResponse> persons_list_from_get = await _personService.GetAllPersons();

            //Assert
            //foreach (PersonResponse person_response_from_add in person_responses_from_add)
            //{
            //    Assert.Contains(person_response_from_add, persons_list_from_get);
            //}
            persons_list_from_get.Should().BeEquivalentTo(person_responses_from_add);
        }
        #endregion
        #region GetFilteredPersons
        //If the search test is empty and search by is "PersonName", it should return all persons 
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText()
        {
            //Arrange
            CountryAddRequest country_request_1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest country_request_2 = _fixture.Create<CountryAddRequest>();
            CountryResponse country_response_1 = await _countryService.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countryService.AddCountry(country_request_2);
            PersonAddRequest person_request_1 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "someone_5@example.com")
                .With(temp => temp.CountryID, country_response_1.CountryID)
                .Create();
            PersonAddRequest person_request_2 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "someone_6@example.com")
                .With(temp => temp.CountryID, country_response_2.CountryID)
                .Create();
            PersonAddRequest person_request_3 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "someone_7@example.com")
                .With(temp => temp.CountryID, country_response_1.CountryID)
                .Create();
            List<PersonAddRequest> person_requests = [person_request_1, person_request_2, person_request_3];
            List<PersonResponse> person_response_from_add = [];
            foreach (PersonAddRequest person in person_requests)
            {
                PersonResponse person_response =await _personService.AddPerson(person);
                person_response_from_add.Add(person_response);
            }
            //print person_response_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response in person_response_from_add)
            {
                _testOutputHelper.WriteLine(person_response.ToString());
            }

            //Act 
            List<PersonResponse> persons_list_from_search =await _personService.GetFilteredPersons(searchBy:nameof(Person.PersonName), searchString: "");
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_list in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_list.ToString());
            }
            ////Assert
            //foreach (PersonResponse person_response in person_response_from_add)
            //{
            //    Assert.Contains(person_response, persons_list_from_search);
            //}
            persons_list_from_search.Should().BeEquivalentTo(person_response_from_add);
        }
        //First we will add few persons; and then we will search based on person name wit some search string. It should
        //return the matching persons

        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName()
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
            CountryResponse country_response_1 =await _countryService.AddCountry(country_request_1);
            CountryResponse country_response_2 =await _countryService.AddCountry(country_request_2);
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
                PersonResponse person_response =await _personService.AddPerson(person);
                person_response_from_add.Add(person_response);
            }
            //print person_response_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response in person_response_from_add)
            {
                _testOutputHelper.WriteLine(person_response.ToString());
            }

            //Act 
            List<PersonResponse> persons_list_from_search = await _personService.GetFilteredPersons(searchBy: nameof(Person.PersonName), searchString: "mu");
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_list in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_list.ToString());
            }
            //Assert
            //foreach (PersonResponse person_response in person_response_from_add)
            //{
            //    if (person_response.PersonName != null)
            //    {
            //        if (person_response.PersonName.Contains("mu", StringComparison.OrdinalIgnoreCase))
            //        {
            //            Assert.Contains(person_response, persons_list_from_search);

            //        }
            //    }
            //}
            persons_list_from_search.Should().OnlyContain(temp => temp.PersonName.Contains("mu", StringComparison.OrdinalIgnoreCase));
        }

        #endregion
        #region GetSortedPersons
        // When we sort based on PersonName in DESC, it should return persons list in descinding on PersonName
        [Fact]
        public async Task GetSortedPersons_DESC()
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
            CountryResponse country_response_1 =await _countryService.AddCountry(country_request_1);
            CountryResponse country_response_2 =await _countryService.AddCountry(country_request_2);
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
                PersonResponse person_response =await _personService.AddPerson(person);
                person_response_from_add.Add(person_response);
            }
            //print person_response_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response in person_response_from_add)
            {
                _testOutputHelper.WriteLine(person_response.ToString());
            }
            List<PersonResponse> allPersons =await _personService.GetAllPersons();
            //Act 
            List<PersonResponse> persons_list_from_sort=await _personService.GetSortedPersons(allPersons,nameof(Person.PersonName),SortOrderOptions.DESC);
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_list in persons_list_from_sort)
            {
                _testOutputHelper.WriteLine(person_response_from_list.ToString());
            }
            //person_response_from_add = person_response_from_add.OrderByDescending(person => person.PersonName).ToList();

            //Assert
            // for(int i = 0; i < person_response_from_add.Count; i++)
            //{
            //    Assert.Equal(person_response_from_add[i], persons_list_from_sort[i]);
            //}
            //persons_list_from_sort.Should().BeEquivalentTo(person_response_from_add);
            persons_list_from_sort.Should().BeInDescendingOrder(temp => temp.PersonName);
        }
        [Fact]
        public async Task GetSortedPersons_ASC()
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
            CountryResponse country_response_1 =await _countryService.AddCountry(country_request_1);
            CountryResponse country_response_2 =await _countryService.AddCountry(country_request_2);
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
                PersonResponse person_response =await _personService.AddPerson(person);
                person_response_from_add.Add(person_response);
            }
            //print person_response_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response in person_response_from_add)
            {
                _testOutputHelper.WriteLine(person_response.ToString());
            }
            List<PersonResponse> allPersons = await _personService.GetAllPersons();
            //Act 
            List<PersonResponse> persons_list_from_sort =await _personService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.ASC);
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_list in persons_list_from_sort)
            {
                _testOutputHelper.WriteLine(person_response_from_list.ToString());
            }
            //person_response_from_add = person_response_from_add.OrderBy(person => person.PersonName).ToList();

            //Assert
            //for (int i = 0; i < person_response_from_add.Count; i++)
            //{
            //    Assert.Equal(person_response_from_add[i], persons_list_from_sort[i]);
            //}
            persons_list_from_sort.Should().BeInAscendingOrder(temp => temp.PersonName);
        }
        #endregion
        #region UpdatePerson

        // When we supply null as PersonUpdateRequest, it should throw ArgumentNullException
        [Fact]
        public async Task UpdatePerson_NullPerson()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = null;

            Func<Task> action = async () =>
            {
                //Act
                await _personService.UpdatePerson(personUpdateRequest);
            };
            //Assert
           await action.Should().ThrowAsync<ArgumentNullException>();


           //await Assert.ThrowsAsync<ArgumentNullException>(async() =>
           // {
           //     //Act 
           //  await   _personService.UpdatePerson(personUpdateRequest);
           // });
        }

        // When we supply invalid personID, it should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_InvalidPersonID()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = new()
            {
                PersonID = Guid.NewGuid()
            };
           Func<Task> action = async () =>
            {
                //Act
                await _personService.UpdatePerson(personUpdateRequest);
            };
            //Assert
           await action.Should().ThrowAsync<ArgumentException>();
        }
        // When the personName is null, it should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_NullPersonName()
        {
            //Arrange
            CountryAddRequest country_add_request = new()
            {
                CountryName = "UK"
            };
            CountryResponse country_response_from_add=await  _countryService.AddCountry(country_add_request);

            PersonAddRequest person_add_request = new()
            {
                PersonName = "John",
                CountryID = country_response_from_add.CountryID,
                Email="John@example.com",
                Address="Address...",
                Gender=GenderOptions.Male
            };
            PersonResponse person_response_from_add =await _personService.AddPerson(person_add_request);
            PersonUpdateRequest? personUpdateRequest = person_response_from_add.ToPersonUpdateRequest();
            personUpdateRequest.PersonName = null;
            Func<Task> action = async () =>
            {
                //Act 
                await _personService.UpdatePerson(personUpdateRequest);
            };
            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }
        // First, add a new person and try to update the same
        [Fact]
        public async Task UpdatePerson_PersonFullDetails()
        {
            //Arrange
            CountryAddRequest country_add_request = new()
            {
                CountryName = "UK"
            };
            CountryResponse country_response_from_add= await _countryService.AddCountry(country_add_request);

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
            PersonResponse person_response_from_add=await _personService.AddPerson(person_add_request);
            PersonUpdateRequest? personUpdateRequest = person_response_from_add.ToPersonUpdateRequest();
            personUpdateRequest.PersonName = "William";
            personUpdateRequest.Email = "william@example.com";

            //Act 
             PersonResponse person_response_from_update=await _personService.UpdatePerson(personUpdateRequest);
            PersonResponse? person_response_from_get=await _personService.GetPersonByPersonID(person_response_from_update.PersonID);

            //Assert
            person_response_from_update.Should().Be(person_response_from_get);
            
        }
        #endregion
        #region DeletePerson
        //If you supply a valid PersonID, it should return true;
        [Fact]
        public async Task DeletePerson_ValidPersonID()
        {
            //Arrange
            CountryAddRequest country_add_request = new()
            {
                CountryName = "USA"
            };
            CountryResponse country_response_from_add =await _countryService.AddCountry(country_add_request);
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
            PersonResponse person_response_from_add =await _personService.AddPerson(person_add_request);

            //Act 
            bool isDeleted=await _personService.DeletePerson(person_response_from_add.PersonID);
            //Assert
            isDeleted.Should().BeTrue();
        }
        //If you supply an invalid PersonID, it should return false;
        [Fact]
        public async Task DeletePerson_InvalidPersonID()
        {
         

            //Act 
            bool isDeleted=await _personService.DeletePerson(Guid.NewGuid());
            //Assert
            //Assert.False(isDeleted);
            isDeleted.Should().BeFalse();

        }
        #endregion
    }
}
