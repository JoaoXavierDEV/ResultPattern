using ResultPattern.Entity;
using ResultPattern.Validation;

namespace ResultPattern.Service
{
    public class UsuarioService
    {
        /// <summary>
        /// Registra o usuário com validação básica.
        /// </summary>
        /// <returns></returns>
        public Result RegisterUser(Usuario user)
        {
            if (string.IsNullOrWhiteSpace(user.Email))
                return Result.Fail(UsuarioValidade.InvalidEmail);

            if (string.IsNullOrWhiteSpace(user.Senha))
                return Result.Fail<Usuario>(UsuarioValidade.InvalidPassword);

            if (UserExists(user.Email))
                return Result.Fail(UsuarioValidade.DuplicateEmail);

            var testeSenha = Result<Usuario>.Validate(
                user => user.Senha.Length < 10,
                "Senha menor que 10 caracteres",
                user
            );

            if (!testeSenha.IsSuccess)
                return testeSenha;

            // Registration logic here
            CreateUser(user);

            return Result<Usuario>.Ok();
        }
        /// <summary>
        /// Registra o usuário com validação e retorno de mensagens de erro.
        /// </summary>
        /// <returns></returns>
        public Result RegisterUser2(Usuario user)
        {
            var validationResult = Result<Usuario>.Create(user);
            var validationResult2 = Result.Create(user);

            if (string.IsNullOrWhiteSpace(user.Email))
                validationResult.AddMessageError(UsuarioValidade.InvalidEmail);

            if (string.IsNullOrWhiteSpace(user.Senha))
                validationResult.AddMessageError(UsuarioValidade.InvalidPassword);

            // Check for duplicate email (simplified example)
            if (UserExists(user.Email))
                validationResult.AddMessageError(UsuarioValidade.DuplicateEmail);

            // Registration logic here
            if (validationResult.IsSuccess)
            {
                CreateUser(user);
            }

            return validationResult;
        }

        public bool UserExists(string email)
        {
            return false;
            // Simulate a check for existing user
            int num = DateTime.Now.Millisecond;

            return num % 2 == 0;
        }

        public void CreateUser(Usuario user)
        {
            // Simulate user creation logic
            Console.WriteLine($"User created with email: {user.Email}");
        }
    }
}
