using ResultPattern.Entity;
using ResultPattern.Validation;

namespace ResultPattern.Tests
{
    public class UnitTest1
    {
        [Fact(DisplayName = "Test OK Result")]
        public void TestOK()
        {
            var resultOK = Result.Ok<Usuario>();

            Assert.IsType<Result<Usuario>>(resultOK);

            Assert.True(resultOK.IsSuccess);

            Assert.Empty(resultOK.Errors);
        }

        [Fact(DisplayName = "Test OK Result with Value")]
        public void TestOk2()
        {
            var resultOK = Result.Ok(new Usuario());
            Assert.IsType<Result<Usuario>>(resultOK);

            Assert.NotNull(resultOK.Value);
            Assert.IsType<Usuario>(resultOK.Value);

            Assert.True(resultOK.IsSuccess);
            Assert.Empty(resultOK.Errors);
        }

        [Fact(DisplayName = "Test Create Result")]
        public void Test2()
        {
            var resultOK = Result.Create<Usuario>(new Usuario());

            Assert.IsType<Result<Usuario>>(resultOK);

            Assert.True(resultOK.IsSuccess);
            Assert.Empty(resultOK.Errors);
        }

        [Fact(DisplayName = "Test Create Result with Value")]
        public void Test3()
        {
            var user = new Usuario();

            var resultOK = Result.Create<Usuario>(user);
            var resultOK2 = Result.Create(user);

            Assert.Equivalent(resultOK, resultOK2);

            Assert.IsType<Result<Usuario>>(resultOK);

            Assert.True(resultOK.IsSuccess);
            Assert.Empty(resultOK.Errors);
        }

        [Fact(DisplayName = "Test Fail Result")]
        public void TestFail()
        {
            var resultOK = Result.Create<Usuario>();

            resultOK.AddMessageError("Um valor nulo foi fornecido.");

            Assert.IsType<Result<Usuario>>(resultOK);

            Assert.True(resultOK.IsFailure);

            Assert.False(resultOK.IsSuccess);

            Assert.NotEmpty(resultOK.Errors);

            Assert.Contains("Um valor nulo foi fornecido.", resultOK.Errors);
        }

        [Fact(DisplayName = "Test Fail Result with Value")]
        public void TestFailWithValue()
        {
            var user = new Usuario();

            var resultOK = Result.Create(user);
            resultOK.AddMessageError("Um valor nulo foi fornecido.");

            var resultOKtyped = Result.Create<Usuario>(user);
            resultOKtyped.AddMessageError("Um valor nulo foi fornecido.");

            Assert.Equivalent(resultOKtyped, resultOK);


            Assert.IsType<Result<Usuario>>(resultOK);

            Assert.True(resultOK.IsFailure);

            Assert.False(resultOK.IsSuccess);

            Assert.NotEmpty(resultOK.Errors);

            Assert.Contains("Um valor nulo foi fornecido.", resultOK.Errors);
        }

        //[Fact(DisplayName = "Test Create Result with Value")]
        //public void Test4()
        //{
        //    new Result<Usuario>();
        //    var resultOK = Result.Create<Usuario>(new Usuario());
        //    Assert.IsType<Result<Usuario>>(resultOK);
        //    Assert.True(resultOK.IsSuccess);
        //    Assert.Empty(resultOK.Errors);

        //}
    }
}