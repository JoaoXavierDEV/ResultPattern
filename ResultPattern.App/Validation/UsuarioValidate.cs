namespace ResultPattern.Validation
{
    public record UsuarioValidade : Error
    {
        public static UsuarioValidade InvalidEmail = new("Error.InvalidEmail", new Dictionary<string, string>
        {
            { "pt-br", "O e-mail fornecido é inválido." },
            { "en-us", "The provided email is invalid." }
        });

        public static UsuarioValidade InvalidPassword = new("Error.InvalidPassword", new Dictionary<string, string>
        {
            { "pt-br", "A senha fornecida é inválida." },
            { "en-us", "The provided password is invalid." }
        });

        public static UsuarioValidade DuplicateEmail = new("Error.DuplicateEmail", new Dictionary<string, string>
        {
            { "pt-br", "Já existe uma conta com este e-mail." },
            { "en-us", "An account with this email already exists." }
        });

        public UsuarioValidade(string code, Dictionary<string, string> messages) : base(code, messages)
        {
        }
    }
}
