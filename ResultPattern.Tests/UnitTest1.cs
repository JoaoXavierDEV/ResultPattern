using ResultPattern.Entity;

namespace ResultPattern.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void TestOK()
        {
            var resultOK = Result.Ok<Usuario>();

            Assert.IsType<Result<Usuario>>(resultOK);

            Assert.Throws<InvalidOperationException>(() => resultOK.Value);

            Assert.True(resultOK.IsSuccess);
            Assert.Empty(resultOK.Errors);
        }

        [Fact]
        public void TestOk2()
        {
            var resultOK = Result.Ok(new Usuario());
            Assert.IsType<Result<Usuario>>(resultOK);

            Assert.NotNull(resultOK.Value);
            Assert.IsType<Usuario>(resultOK.Value);

            Assert.True(resultOK.IsSuccess);
            Assert.Empty(resultOK.Errors);
        }

        [Fact]
        public void Test2()
        {
            var resultOK = Result.Create<Usuario>(new Usuario());

            Assert.IsType<Result<Usuario>>(resultOK);

            Assert.True(resultOK.IsSuccess);
            Assert.Empty(resultOK.Errors);
        }

        [Fact]
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

        [Fact]
        public void TestFail()
        {
            var user = new Usuario();

            var resultOK = Result.Create<Usuario>(null);

            Assert.Throws<InvalidOperationException>(() => resultOK.Value);

            Assert.IsType<Result<Usuario>>(resultOK);

            Assert.True(resultOK.IsFailure);
            Assert.False(resultOK.IsSuccess);

            Assert.NotEmpty(resultOK.Errors);

            Assert.Contains("Um valor nulo foi fornecido.", resultOK.Errors);
        }




    }
}