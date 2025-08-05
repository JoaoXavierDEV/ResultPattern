namespace ResultPattern.Validation
{
    public record ProdutoValidate : Error
    {
        protected ProdutoValidate(string code, Dictionary<string, string> messages) : base(code, messages)
        {
        }

        public static ProdutoValidate PrecoNegativo = new("Produto.PrecoNegativo", new Dictionary<string, string>
        {
            { "pt-br", "O preço deve ser maior que zero." },
            { "en-us", "Price must be greater than zero." }
        });
    }
}
