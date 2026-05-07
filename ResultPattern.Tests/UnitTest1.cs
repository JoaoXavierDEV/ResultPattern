using ResultPattern.Entity;
using ResultPattern.Validation;

namespace ResultPattern.Tests
{
    public class UnitTest1
    {
        [Fact(DisplayName = "Test OK Result<T>")]
        public void TestOK()
        {
            var resultOK = Result.Ok(new Usuario());

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

        [Fact(DisplayName = "Test Create<T> Result")]
        public void Test2()
        {
            var resultOK = Result.Create(new Usuario());

            Assert.IsType<Result<Usuario>>(resultOK);

            Assert.True(resultOK.IsSuccess);
            Assert.Empty(resultOK.ValidationErrors);
        }

        [Fact(DisplayName = "Test Create Result with Value")]
        public void Test3()
        {
            var user = new Usuario();

            var resultOK = Result.Ok<Usuario>(user);
            var resultOK2 = Result.Ok(user);

            Assert.Equivalent(resultOK, resultOK2);

            Assert.IsType<Result<Usuario>>(resultOK);

            Assert.True(resultOK.IsSuccess);
            Assert.Empty(resultOK.ValidationErrors);
        }

        [Fact(DisplayName = "Test Fail")]
        public void TestFail()
        {
            var resultOK = Result.Fail<Usuario>(x => x.Email, UsuarioValidade.InvalidEmail, new Usuario(), "en_us");

            resultOK.AddMessageError(x => x.Email, "Um valor nulo foi fornecido.");

            Assert.IsType<Result<Usuario>>(resultOK);

            Assert.True(resultOK.IsFailure);

            Assert.False(resultOK.IsSuccess);

            Assert.NotEmpty(resultOK.ValidationErrors);

            Assert.Contains(resultOK.ValidationErrors, kv => kv.Value.Contains("Um valor nulo foi fornecido."));
        }

        [Fact(DisplayName = "Test Create with AddMessageError")]
        public void TestCreateWithAddMessageError()
        {
            var resultOK = Result.Ok(new Usuario());

            resultOK.AddMessageError(x => x.Email, "Um valor nulo foi fornecido.");

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

            var resultOK = Result.Ok(user);
            resultOK.AddMessageError(x => x.Email, "Um valor nulo foi fornecido.");

            var resultOKtyped = Result.Ok<Usuario>(user);
            resultOKtyped.AddMessageError(x => x.Email, "Um valor nulo foi fornecido.");

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
            var result = Result.Ok(usuario);

            result.AddMessageError(x => x.Email, "Um valor nulo foi fornecido.");

            result.AddMessageError(x => x.Email, "Um valor nulo foi fornecido.");

            result.AddMessageError(x => x.Senha, "Senha deve ter no mínimo 6 caracteres");

            result.AddMessageError(x => x.Nome, "Nome deve ter no mínimo 3 caracteres");

            result.AddMessageError(x => x.Endereco.Cep, "CEP do usuário não pode ser nulo");

            Assert.Equal(usuario.Email, result.Value.Email);

            


            Assert.IsType<Result<Usuario>>(result);

            Assert.True(result.IsFailure);
            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.ValidationErrors);
        }

        [Fact(DisplayName = "Test Error Count")]
        public void TestErrorCount()
        {
            var usuario = new Usuario
            {
                Nome = string.Empty, // Garante que não será nulo
                Email = "joao@outlook.com",
                Senha = string.Empty
            };
            var result = Result.Ok(usuario);

            result.AddMessageError(x => x.Email, "Um valor nulo foi fornecido.");
            result.AddMessageError(x => x.Senha, "Senha deve ter no mínimo 6 caracteres");
            result.AddMessageError(x => x.Nome, "Nome deve ter no mínimo 3 caracteres");
            result.AddMessageError(x => x.Nome, "Nome não pode ser nulo");

            Assert.Equal(4, result.ErrorCount);
        }

        [Fact(DisplayName = "Test Nested Property Path in ValidationErrors")]
        public void TestNestedPropertyPath()
        {
            var usuario = new Usuario();
            var result = Result.Ok(usuario);

            result.AddMessageError(x => x.Endereco.Cep, "CEP do usuário não pode ser nulo");
            result.AddMessageError(x => x.Endereco.Cidade, "Cidade é obrigatória");
            result.AddMessageError(x => x.Nome, "Nome é obrigatório");

            Assert.True(result.IsFailure);
            Assert.Contains("Usuario.Endereco.Cep", result.ValidationErrors.Keys);
            Assert.Contains("Usuario.Endereco.Cidade", result.ValidationErrors.Keys);
            Assert.Contains("Usuario.Nome", result.ValidationErrors.Keys);
            Assert.Contains("CEP do usuário não pode ser nulo", result.ValidationErrors["Usuario.Endereco.Cep"]);
            Assert.Contains("Cidade é obrigatória", result.ValidationErrors["Usuario.Endereco.Cidade"]);
        }
    }
}