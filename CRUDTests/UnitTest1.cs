using CRUDTests.Services;
using Xunit;
namespace CRUDTests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            //Arange : the decleration of variables and collecting the inputs 

            MyMath mm = new();
            int input1 = 10, input2 = 5;
            int expected = 15;

            //Act : calling the methods
            int actual=mm.Add(input1, input2);

            //Assert : comparing the expected value with actual value (Pass | Fail)
            Assert.Equal(expected, actual);

        }
    }
}
