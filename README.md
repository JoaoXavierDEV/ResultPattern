# ResultPattern

Este projeto implementa o padrão **Result Pattern** com suporte a mensagens de erro em múltiplos idiomas, visando facilitar a internacionalização e melhorar a experiência do usuário.

## Sobre o projeto

O **Result Pattern** é uma abordagem alternativa ao uso de exceções para validação e tratamento de erros. Em vez de lançar exceptions, os métodos retornam objetos que indicam sucesso ou falha, juntamente com mensagens de erro apropriadas e localizadas conforme o idioma configurado.

### Principais características

- **Validação sem exceptions:** Evita o uso de exceções para controle de fluxo, tornando o código mais previsível e fácil de testar.
- **Mensagens de erro multi-idiomas:** As mensagens de erro são adaptadas ao idioma do usuário, facilitando a internacionalização.

## Estrutura

- `ResultPattern.App\Validation\ResultPattern.cs`: Implementação do padrão Result.
- `ResultPattern.App\Validation\ErrorMessage.cs`: Gerenciamento das mensagens de erro em diferentes idiomas.
- Validações de entidades como `Usuario` e `Produto` utilizando o padrão.

## Quando usar?

Utilize o **Result Pattern** quando desejar evitar o uso de exceções para validação, especialmente em cenários onde o controle de fluxo e a clareza das mensagens de erro são essenciais.

## Exemplo de uso

#### Retorna apenas 1 erro
```csharp
public Result RegisterUser(Usuario user)
{
    // validação usando o record Error
    if (string.IsNullOrWhiteSpace(user.Email))
        return Result.Fail(UsuarioValidade.InvalidEmail);

    if (string.IsNullOrWhiteSpace(user.Senha))
        return Result.Fail<Usuario>(UsuarioValidade.InvalidPassword);

    if (UserExists(user.Email))
        return Result.Fail(UsuarioValidade.DuplicateEmail);

    // validação personalizada
    var testeSenha = Result<Usuario>.Validate(
        user => user.Senha.Length < 10,
        "Senha menor que 10 caracteres",
        user
    );

    if (!testeSenha.IsSuccess)
        return testeSenha;

    CreateUser(user);

    return Result<Usuario>.Ok();
}
```
#### Retorna uma lista de erro 
```csharp
public Result RegisterUser2(Usuario user)
{
    var validationResult = Result<Usuario>.Create(user);

    if (string.IsNullOrWhiteSpace(user.Email))
        validationResult.AddMessageError(UsuarioValidade.InvalidEmail);

    if (string.IsNullOrWhiteSpace(user.Senha))
        validationResult.AddMessageError(UsuarioValidade.InvalidPassword);

    if (UserExists(user.Email))
		validationResult.AddMessageError(UsuarioValidade.DuplicateEmail);

    if (validationResult.IsSuccess)
        CreateUser(user);    

    return validationResult;
}
```

![Pode receber uma ou várias mensagens de erro](https://raw.githubusercontent.com/JoaoXavierDEV/ResultPattern/refs/heads/master/Docs/watchptbr2erros.png)

![Mensagens de error conforme o idioma](https://raw.githubusercontent.com/JoaoXavierDEV/ResultPattern/refs/heads/master/Docs/watchEnUS2erros.png)

```csharp
public record Error
{
    public string Code { get; }
    public Dictionary<string, string> Messages { get; }
    public Error(string code, Dictionary<string, string> messages)
    {
        Code = code;
        Messages = messages;
    }
	public static Error NullValue = new("Error.NullValue", new Dictionary<string, string>
	{
		{ "pt-br", "Um valor nulo foi fornecido." },
		{ "en-us", "A null value was provided." }
	});
}

public record UsuarioValidade : Error
{
	public static UsuarioValidade InvalidEmail = new("Error.InvalidEmail", new Dictionary<string, string>
	{
	    { "pt-br", "O e-mail fornecido é inválido." },
	    { "en-us", "The provided email is invalid." }
	});
}
```
---
Projeto desenvolvido em .NET 8.