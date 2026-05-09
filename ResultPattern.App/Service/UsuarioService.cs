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
            // validação usando o record Error
            if (string.IsNullOrWhiteSpace(user.Email))
                return Result.Fail<Usuario>(x => x.Email, UsuarioValidade.InvalidEmail, user);

            if (string.IsNullOrWhiteSpace(user.Senha))
                return Result.Fail<Usuario>(x => x.Senha, UsuarioValidade.InvalidPassword, user);

            if (UserExists(user.Email))
                return Result.Fail<Usuario>(x => x.Email, UsuarioValidade.DuplicateEmail, user);

            // validação personalizada
            var testeSenha = Result<Usuario>.Validate(
                u => u.Senha.Length < 10,
                x => x.Senha,
                UsuarioValidade.InvalidPassword,
                user
            );

            if (!testeSenha.IsSuccess)
                return testeSenha;

            CreateUser(user);

            return Result.Ok(user);
        }
        /// <summary>
        /// Registra o usuário com validação e retorno de mensagens de erro.
        /// </summary>
        /// <returns></returns>
        public Result<Usuario> RegisterUser2(Usuario user)
        {
            var validationResult = Result.Create(user);

            if (string.IsNullOrWhiteSpace(user.Email))
                validationResult.AddMessageError(x => x.Email, UsuarioValidade.InvalidEmail);

            if (UserExists(user.Email))
                validationResult.AddMessageError(x => x.Email, UsuarioValidade.DuplicateEmail);

            if (string.IsNullOrWhiteSpace(user.Senha))
                validationResult.AddMessageError(x => x.Senha, UsuarioValidade.InvalidPassword);

            var testeSenha = Result<Usuario>.Validate(
                u => u.Senha.Length < 8,
                x => x.Senha,
                UsuarioValidade.InvalidPassword,
                user
            );


            if (validationResult.IsSuccess)
            {
                CreateUser(user);
            }

            return validationResult;
        }

        public Result<Usuario> RegisterUser3(Usuario user)
        {
            var validationResult = Result.Ok(user);

            if (string.IsNullOrWhiteSpace(user.Email))
                validationResult.AddMessageError(x => x.Email, UsuarioValidade.InvalidEmail);

            if (string.IsNullOrWhiteSpace(user.Senha))
                validationResult.AddMessageError(x => x.Senha, UsuarioValidade.InvalidPassword);

            if (UserExists(user.Email))
                validationResult.AddMessageError(x => x.Email, UsuarioValidade.DuplicateEmail);

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
        }

        public void CreateUser(Usuario user)
        {
            // Simulate user creation logic
            Console.WriteLine($"User created with email: {user.Email}");
        }
    }
}
