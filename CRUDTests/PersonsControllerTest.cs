using AutoFixture;
using Moq;
using ServiceContracts;
using FluentAssertions;
using CRUDExample.Controllers;
using ServiceContracts.DTO; 
using ServiceContracts.Enums;
using Microsoft.AspNetCore.Mvc; 
using Microsoft.Extensions.Logging;
namespace CRUDTests
{
    public class PersonsControllerTest
    {
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;
        private readonly ILogger<PersonsController> _logger;
        private readonly Mock<ICountriesService> _countriesServiceMock;
        private readonly Mock<IPersonsService> _personsServiceMock;
        private readonly Mock<ILogger<PersonsController>> _loggerMock;
        private readonly IFixture _fixture;
         
        public PersonsControllerTest()
        {
            _fixture = new Fixture();

            _personsServiceMock = new Mock<IPersonsService>();
            _personsService = _personsServiceMock.Object;
            _loggerMock = new Mock<ILogger<PersonsController>>();


            _countriesServiceMock = new Mock<ICountriesService>();
            _countriesService = _countriesServiceMock.Object;
            _logger = _loggerMock.Object;

            
        }

        #region Index
        [Fact]
        public async Task Index_ShouldReturnIndexViewWithPersonsList()
        {
            //Arrange 
            List<PersonResponse> persons_response_list = _fixture.Create<List<PersonResponse>>();
            PersonsController personsController = new PersonsController(_personsService, _countriesService,_logger);

            _personsServiceMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(persons_response_list);

            _personsServiceMock.Setup(temp => temp.GetSortedPersons(It.IsAny<List<PersonResponse>>(),It.IsAny<string>(),It.IsAny<SortOrderOptions>()))
                .ReturnsAsync(persons_response_list);
            //Act 
            IActionResult result =await personsController.Index(
                                          searchBy: _fixture.Create<string>(),
                                          searchString: _fixture.Create<string>(),
                                          sortBy: _fixture.Create<string>(),
                                          sortOrder: _fixture.Create<SortOrderOptions>());
            //Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
            viewResult.ViewData.Model.Should().Be(persons_response_list);
        }

        #endregion
        #region Create
        
        //[Fact]
        //public async Task Create_IfModelErrors_ToReturnCreateView()
        //{
        //    //Arrange 
        //    PersonAddRequest person_add_request = _fixture.Create<PersonAddRequest>();
        //    PersonResponse person_response = _fixture.Create<PersonResponse>();

        //    List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();
            

        //    _countriesServiceMock.Setup(temp => temp.GetAllCountries())
        //        .ReturnsAsync(countries);

        //    _personsServiceMock.Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>()))
        //         .ReturnsAsync(person_response);
        //    PersonsController personsController = new(_personsService, _countriesService,_logger);
        //    //Act 
        //    //Add model error
        //    personsController.ModelState.AddModelError("PersonName", "Person Name can't be blank");
        //    IActionResult result = await personsController.Create(personRequest:person_add_request);

        //    //Assert
        //    ViewResult viewResult = Assert.IsType<ViewResult>(result);
        //    viewResult.ViewData.Model.Should().BeAssignableTo<PersonAddRequest>();
        //    viewResult.ViewData.Model.Should().Be(person_add_request);
        //}
        [Fact]
        public async Task Create_IfNoModelErrors_ToRedirectToIndex()
        {
            //Arrange 
            PersonAddRequest person_add_request = _fixture.Create<PersonAddRequest>();
            PersonResponse person_response = _fixture.Create<PersonResponse>();

            List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();
            PersonsController personsController = new(_personsService, _countriesService,_logger);

            _countriesServiceMock.Setup(temp => temp.GetAllCountries())
                .ReturnsAsync(countries);

            _personsServiceMock.Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>()))
                 .ReturnsAsync(person_response);

            //Act 
            IActionResult result = await personsController.Create(personRequest: _fixture.Create<PersonAddRequest>());

            //Assert
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
            redirectResult.ActionName.Should().Be("Index");
            redirectResult.ControllerName.Should().Be("Persons");
        }
        #endregion
    }
}
