using System.Globalization;

namespace ResultPattern.Validation
{
    public record Error
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
            if (language != null && language != string.Empty)
            {
                language = language.ToLower();
            }

            language ??= CultureInfo.CurrentCulture.Name.ToLower();

            if (Messages.TryGetValue(language, out string? message))
                return message;

            // Fallback para inglês se não encontrar o idioma solicitado
            if (Messages.TryGetValue("en-us", out string? defaultMessage))
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


    }
}
