using AutoFixture;
 using Entities;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System.Linq.Expressions;
using Xunit.Abstractions;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personService;
        private readonly Mock<IPersonsRepository> _personRepositoryMock;
        private readonly IPersonsRepository _personsRepository;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;
        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture();

            _personRepositoryMock = new Mock<IPersonsRepository>();
            _personsRepository = _personRepositoryMock.Object;

            _personService = new PersonsService(_personsRepository );
             _testOutputHelper = testOutputHelper;
         }
        #region AddPerson
        //When we supply null value as PersonAddRequest, it should throw ArgumentNullException
        [Fact]
        public async Task AddPerson_NullPerson_ToBeArgumentException()
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
        public async Task AddPerson_PersonNameIsNull_ToBeArgumentException()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, null as string)
                .Create();

            Person person = personAddRequest.ToPerson();

            //When PersonsRepository.AddPerson is called, it has to return the same "person" object

            _personRepositoryMock.Setup(r => r.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);
            //Act
            Func<Task> action = async () =>
            {
                await _personService.AddPerson(personAddRequest);
            };

            //Assertion
            await action.Should().ThrowAsync<ArgumentException>();
           
        }
        //When we supply proper PersonDetails , it should insert the person into the persons list
        [Fact]
         public async Task AddPerson_FullPersonDetails_ToBeSuccessful()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture
                .Build<PersonAddRequest>()
                .With(temp => temp.Email,"someone@gmail.com")
                .Create();
            Person person = personAddRequest.ToPerson();
            PersonResponse person_response_expected = person.ToPersonResponse();
            //Act 
            //If we supply any argument value to the AddPerson method,
            //it should return the same return value.
            _personRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);

            PersonResponse person_response_from_add= await _personService.AddPerson(personAddRequest);
            person_response_expected.PersonID = person_response_from_add.PersonID;
            //Assert
            person_response_from_add.PersonID.Should().NotBe(Guid.Empty);

            person_response_from_add.Should().BeEquivalentTo(person_response_expected);

            
        }
        #endregion
        #region GetPersonByPersonID
        //If we supply null as personID, it should return null as PersonResponse
        [Fact]
        public async Task GetPersonByPersonID_NullPersonID_ToBeNull()
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
        public async Task GetPersonByPersonID_WithPersonID_ToBeSuccessful()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
                .With(temp => temp.Email, "email@sample.com")
                .With(temp => temp.Country,null as Country)
                .Create();
            PersonResponse person_response_expected = person.ToPersonResponse();

            _personRepositoryMock.Setup(r => r.GetPersonByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(person);
            //Act
            PersonResponse? person_response_from_get= await _personService.GetPersonByPersonID(person.PersonID);
            //Assert
            person_response_from_get.Should().Be(person_response_expected);
            //Assert.Equal(person_response_from_add, person_response_from_get);
        }
        #endregion
        #region GetAllPersons 

        // the GetAllPersons() should return an empty list by default 
        [Fact]
        public async Task GetAllPersons_EmptyList_ToBeEmptyList()
        {
            //Arrange
            var persons = new List<Person>();
            _personRepositoryMock.Setup(r => r.GetAllPersons())
                .ReturnsAsync(persons);
            //Act 
            List<PersonResponse> persons_response_from_get= await _personService.GetAllPersons();
            //Assert
            persons_response_from_get.Should().BeEmpty();
        }
        // First, we will add few persons; and then when we call GetAllPersons(), it should return the same persons were added
        [Fact]
        public async Task GetAllPersons_WithFewPersons_ToBeSuccessful()
        {
            //Arrange
            List<Person> persons =
            [
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_3@example.com")
                .With(temp => temp.Country , null as Country)
                .Create(),
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_4@example.com")
                .With(temp => temp.Country , null as Country)
                .Create(),
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_5@example.com")
                .With(temp => temp.Country , null as Country)
                .Create(),
            ];
            List<PersonResponse> person_response_list_expected = [.. persons.Select(temp => temp.ToPersonResponse())];

            _testOutputHelper.WriteLine("Expected:");
            foreach(PersonResponse person_response_from_add in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }
            _personRepositoryMock.Setup(temp => temp.GetAllPersons())
                .ReturnsAsync(persons);
            //Act
            List<PersonResponse> persons_list_from_get = await _personService.GetAllPersons();

            //Print persons_list_from_get
            _testOutputHelper.WriteLine("Actual:");
            foreach(PersonResponse person_response_from_get in persons_list_from_get)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }
            //Assert
            persons_list_from_get.Should().BeEquivalentTo(person_response_list_expected);

            
        }
        #endregion
        #region GetFilteredPersons
        //If the search test is empty and search by is "PersonName", it should return all persons 
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText_ToBeSuccessful()
        {
            //Arrange
            List<Person> persons =
              [
             _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_3@example.com")
                .With(temp => temp.Country , null as Country)
                .Create(),
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_4@example.com")
                .With(temp => temp.Country , null as Country)
                .Create(),
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_5@example.com")
                .With(temp => temp.Country , null as Country)
                .Create(),
             ];
            List<PersonResponse> person_response_list_expected = [.. persons.Select(p => p.ToPersonResponse())];
            //print person_response_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_response.ToString());
            }

            _personRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
                .ReturnsAsync(persons);
            //Act 
            List<PersonResponse> persons_list_from_search =await _personService.GetFilteredPersons(searchBy:nameof(Person.PersonName), searchString: "");
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_list in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_list.ToString());
            }
            ////Assert
            
            persons_list_from_search.Should().BeEquivalentTo(person_response_list_expected);
        }

        //First we will add few persons; and then we will search based on person name wit some search string. It should
        //return the matching persons

        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName_ToBeSuccessful()
        {
            //Arrange
            List<Person> persons = [
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_3@example.com")
                .With(temp => temp.Country , null as Country)
                .Create(),
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_4@example.com")
                .With(temp => temp.Country , null as Country)
                .Create(),
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_5@example.com")
                .With(temp => temp.Country , null as Country)
                .Create(),
                 ];

            List<PersonResponse> person_response_list_expected = [.. persons.Select(p => p.ToPersonResponse())];

            //Print person_response_list_expected
            _testOutputHelper.WriteLine("Expected:");
            foreach( PersonResponse person_response in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_response.ToString());
            }

            _personRepositoryMock.Setup(r => r.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
                .ReturnsAsync(persons);
            //Act 
            List<PersonResponse> persons_list_from_search = await _personService.GetFilteredPersons(nameof(PersonResponse.PersonName), "sa");
            //Print persons_list_from_search
            _testOutputHelper.WriteLine("Actual:");
            foreach(PersonResponse person_response in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response.ToString());
            }
            //Assert
            persons_list_from_search.Should().BeEquivalentTo(person_response_list_expected);

            }
        #endregion
        #region GetSortedPersons
        // When we sort based on PersonName in DESC, it should return persons list in descinding on PersonName
        [Fact]
        public async Task GetSortedPersons_DESC_ToBeSuccessful()
        {

            //Arrange
            List<Person> persons =
              [
             _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_3@example.com")
                .With(temp => temp.Country , null as Country)
                .Create(),
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_4@example.com")
                .With(temp => temp.Country , null as Country)
                .Create(),
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_5@example.com")
                .With(temp => temp.Country , null as Country)
                .Create(),
             ];

            List<PersonResponse> person_response_list_expected = [.. persons.Select(temp => temp.ToPersonResponse())];

            _personRepositoryMock.Setup(temp => temp.GetAllPersons())
                .ReturnsAsync(persons);
             
            //print person_response_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response in person_response_list_expected)
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
            persons_list_from_sort.Should().BeInDescendingOrder(temp => temp.PersonName);
        }
        [Fact]
        public async Task GetSortedPersons_ASC()
        {

            //Arrange
            List<Person> persons =
              [
             _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_3@example.com")
                .With(temp => temp.Country , null as Country)
                .Create(),
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_4@example.com")
                .With(temp => temp.Country , null as Country)
                .Create(),
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_5@example.com")
                .With(temp => temp.Country , null as Country)
                .Create(),
             ];

            List<PersonResponse> person_response_list_expected = [.. persons.Select(temp => temp.ToPersonResponse())];

            _personRepositoryMock.Setup(temp => temp.GetAllPersons())
                .ReturnsAsync(persons);

            //print person_response_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response in person_response_list_expected)
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
        public async Task UpdatePerson_NullPerson_ToBeArgumentNullException()
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

        }

        // When we supply invalid personID, it should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_InvalidPersonID_ToBeArgumentException()
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
        public async Task UpdatePerson_PersonNameIsNull_ToBeArgumentException()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
                 .With(p => p.Country, null as Country)
                 .With(p => p.Email,"someone@example.com")
                 .With(p => p.Gender,GenderOptions.Male.ToString())
                 .With(p => p.PersonName, null as string)
                 .Create();
            PersonResponse person_response_from_add = person.ToPersonResponse();
            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
            Func<Task> action = async () =>
            {
                //Act 
                await _personService.UpdatePerson(person_update_request);
            };
            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }
        // First, add a new person and try to update the same
        [Fact]
        public async Task UpdatePerson_PersonFullDetails_ToBeSuccessful()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
                .With(p => p.Email, "someone@example.com")
                .With(p => p.Gender, GenderOptions.Male.ToString())
                .With(p => p.Country, null as Country)
                .Create();
            PersonResponse person_response_expected= person.ToPersonResponse();
            PersonUpdateRequest person_update_request = person_response_expected.ToPersonUpdateRequest();

            _personRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(person);
            _personRepositoryMock.Setup(temp => temp.UpdatePerson(It.IsAny<Person>()))
                .ReturnsAsync(person);

            //Act 
            PersonResponse person_response_from_update=await _personService.UpdatePerson(person_update_request);

            //Assert
            person_response_from_update.Should().Be(person_response_expected);            
        }
        #endregion
        #region DeletePerson
        //If you supply a valid PersonID, it should return true;
        [Fact]
        public async Task DeletePerson_ValidPersonID_ToBeSuccessful()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
                 .With(p => p.Country, null as Country)
                 .With(p => p.PersonName, "Rahman")
                 .With(p => p.Gender, GenderOptions.Male.ToString())
                 .With(p => p.Email, "Rahman@example.com")
                 .Create();

            PersonResponse person_response_expected = person.ToPersonResponse();
            _personRepositoryMock.Setup(temp => temp.DeletePersonByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(true);
            _personRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(person);
            //Act 
            bool isDeleted=await _personService.DeletePerson(person.PersonID);
            //Assert
            isDeleted.Should().BeTrue();
        }
        //If you supply an invalid PersonID, it should return false;
        [Fact]
        public async Task DeletePerson_InvalidPersonID_ToBeFalse()
        {

            //Arrange
            Person person = _fixture.Build<Person>()
                 .With(p => p.Country, null as Country)
                 .With(p => p.PersonName, "Rahman")
                 .With(p => p.Gender, GenderOptions.Male.ToString())
                 .With(p => p.Email, "Rahman@example.com")
                 .Create();

            PersonResponse person_response_expected = person.ToPersonResponse();
            _personRepositoryMock.Setup(temp => temp.DeletePersonByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            //Act 
            bool isDeleted = await _personService.DeletePerson(person.PersonID);
            //Assert
            isDeleted.Should().BeFalse();

        }
        #endregion
    }
}
