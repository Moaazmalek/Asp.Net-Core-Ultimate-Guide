using FluentAssertions;
using Fizzler;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
namespace CRUDTests
{
    public class PersonsControllerIntegrationTest:IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public PersonsControllerIntegrationTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient(); // create http client object
        }
        #region Index

        [Fact]
        public async Task Index_ToReturnView()
        {
            //Arrange

            //Act
            HttpResponseMessage response = await _client.GetAsync("/Persons/Index");
            //Assert
            response.IsSuccessStatusCode.Should().BeTrue();
          string responseBody=await  response.Content.ReadAsStringAsync();

            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(responseBody);
            var document = html.DocumentNode;
            document.QuerySelectorAll("table.persons").Should().NotBeNull();
        }
        #endregion
    }
}
