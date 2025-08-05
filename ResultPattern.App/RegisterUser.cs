using System.Globalization;

namespace ResultPattern
{
    public class Usuario
    {
        public Result RegisterUser(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
                return Result.Fail(Error.InvalidEmail);

            if (string.IsNullOrWhiteSpace(password))
                return Result.Fail(Error.InvalidPassword);

            // Check for duplicate email (simplified example)
            if (UserExists(email))
                return Result.Fail(Error.DuplicateEmail);

            // Registration logic here
            CreateUser(email, password);

            return Result.Ok();
        }

        public Result RegisterUser2(string email, string password)
        {
            var validationResult = Result.Create(email);

            if (string.IsNullOrWhiteSpace(email))
                validationResult.AddMessageError(Error.InvalidEmail);

            if (string.IsNullOrWhiteSpace(password))
                validationResult.AddMessageError(Error.InvalidPassword);

            // Check for duplicate email (simplified example)
            if (UserExists(email))
                validationResult.AddMessageError(Error.DuplicateEmail);

            // Registration logic here
            if (validationResult.IsSuccess)
            {
                CreateUser(email, password);

            }

            return validationResult;
        }

        public bool UserExists(string email)
        {
            // Simulate a check for existing user
            return false; // Assume no user exists for simplicity
        }

        public void CreateUser(string email, string password)
        {
            // Simulate user creation logic
            Console.WriteLine($"User created with email: {email}");
        }
    }
}

public class Error
{
    public string Code { get; }
    public Dictionary<string, string> Messages { get; }

    public Error(string code, Dictionary<string, string> messages)
    {
        Code = code;
        Messages = messages;
    }

    public string GetMessage(string? language = null)
    {
        language ??= CultureInfo.CurrentCulture.Name.ToLower();

        if (Messages.TryGetValue(language, out var message))
            return message;

        // Fallback para inglês se não encontrar o idioma solicitado
        if (Messages.TryGetValue("en-us", out var defaultMessage))
            return defaultMessage;

        // Fallback para a primeira mensagem disponível
        return Messages.Values.FirstOrDefault() ?? string.Empty;
    }

    public static Error None = new(string.Empty, new Dictionary<string, string>());
    public static Error NullValue = new("Error.NullValue", new Dictionary<string, string>
    {
        { "pt-br", "Um valor nulo foi fornecido." },
        { "en-us", "A null value was provided." }
    });
    public static Error InvalidEmail = new("Error.InvalidEmail", new Dictionary<string, string>
    {
        { "pt-br", "O e-mail fornecido é inválido." },
        { "en-us", "The provided email is invalid." }
    });
    public static Error InvalidPassword = new("Error.InvalidPassword", new Dictionary<string, string>
    {
        { "pt-br", "A senha fornecida é inválida." },
        { "en-us", "The provided password is invalid." }
    });
    public static Error DuplicateEmail = new("Error.DuplicateEmail", new Dictionary<string, string>
    {
        { "pt-br", "Já existe uma conta com este e-mail." },
        { "en-us", "An account with this email already exists." }
    });
}


