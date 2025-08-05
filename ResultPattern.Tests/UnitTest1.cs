using ResultPattern.Entity;

namespace ResultPattern.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var resultOK = Result.Ok();

            Assert.True(resultOK.IsSuccess);
            Assert.Empty(resultOK.Errors);
        }

        [Fact]
        public void Test2()
        {
            var resultOK = Result.Create<Usuario>(new Usuario());

            Assert.True(resultOK.IsSuccess);
            Assert.Empty(resultOK.Errors);
        }


    }
}