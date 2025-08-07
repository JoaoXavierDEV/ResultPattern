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

```csharp
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
```

---

Projeto desenvolvido em .NET 8.