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

            Assert.Empty(resultOK.ValidationErrors);
        }

        [Fact(DisplayName = "Test OK Result with Value")]
        public void TestOk2()
        {
            var resultOK = Result.Ok(new Usuario());
            Assert.IsType<Result<Usuario>>(resultOK);

            Assert.NotNull(resultOK.Value);
            Assert.IsType<Usuario>(resultOK.Value);

            Assert.True(resultOK.IsSuccess);
            Assert.Empty(resultOK.ValidationErrors);
        }

        [Fact(DisplayName = "Test Create Result")]
        public void Test2()
        {
            var resultOK = Result.Create<Usuario>(new Usuario());

            Assert.IsType<Result<Usuario>>(resultOK);

            Assert.True(resultOK.IsSuccess);
            Assert.Empty(resultOK.ValidationErrors);
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
            Assert.Empty(resultOK.ValidationErrors);
        }

        [Fact(DisplayName = "Test Fail Result")]
        public void TestFail()
        {
            var resultOK = Result.Create<Usuario>();

            resultOK.AddMessageError("Um valor nulo foi fornecido.");

            Assert.IsType<Result<Usuario>>(resultOK);

            Assert.True(resultOK.IsFailure);

            Assert.False(resultOK.IsSuccess);

            Assert.NotEmpty(resultOK.ValidationErrors);

            Assert.Contains(resultOK.ValidationErrors, kv => kv.Value.Contains("Um valor nulo foi fornecido."));
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

            Assert.NotEmpty(resultOK.ValidationErrors);

            Assert.Contains(resultOK.ValidationErrors, kv => kv.Value.Contains("Um valor nulo foi fornecido."));
        }

        [Fact(DisplayName = "Test Create Result with Value 2")]
        public void Test4()
        {
            var usuario = new Usuario
            {
                Nome = string.Empty, // Garante que não será nulo
                Email = "joao@outlook.com",
                Senha = string.Empty
            };          
            var result = Result.Create<Usuario>(usuario);

            result.AddMessageError(nameof(Usuario.Email), "Um valor nulo foi fornecido.");

            result.AddMessageError(x => x.Email, "Um valor nulo foi fornecido.");

            result.AddMessageError(x => x.Senha, "Senha deve ter no mínimo 6 caracteres");

            result.AddMessageError(x => x.Nome, "Nome deve ter no mínimo 3 caracteres");

            result.AddMessageError(x => x.Nome, "Nome não pode ser nulo");

            Assert.Equal(usuario.Email, result.Value.Email);

            Assert.Equal(5, result.ErrorCount);

            Assert.IsType<Result<Usuario>>(result);

            Assert.True(result.IsFailure);
            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.ValidationErrors);
        }
    }
}