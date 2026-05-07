using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResultPattern;
using ResultPattern.Entity;
using ResultPattern.Service;
using ResultPattern.Validation;

public class Program
{
    private static UsuarioService userService = new UsuarioService();


    public static void Main(string[] args)
    {

        var atual = CultureInfo.CurrentCulture;

        var user = new Usuario
        {
            Nome = "",
            Email = "",
            Senha = ""
        };

        // Retorna um resultado com sucesso ou falha
        var result = userService.RegisterUser3(user);
        //atual = CultureInfo.CurrentCulture;


        CultureInfo.CurrentCulture = new CultureInfo("en-US");

        var user2 = new Usuario
        {
            Nome = "",
            Email = ""
        };

        // Retorna um Result com uma lista de mensagens de erro
        var result2 = userService.RegisterUser2(user2);



        // Exemplo de uso do Match para lidar com o resultado

        var tt = result2.Match(
            () =>
            {
                Console.WriteLine("User registered successfully.");
                return 0;
            },
            errors =>
            {
                Console.WriteLine("Failed to register user:");
                foreach (var error in errors)
                {
                    Console.WriteLine($"- {error}");
                }
                return -1;
            }
        );

        result2.Match2<Usuario>(
            onSuccess: (usuario) =>
            {
                Console.WriteLine("User registered successfully.");
                //return 0;
            },
            onFailure: (errors, usuario) =>
            {
                Console.WriteLine("Failed to register user:");
                foreach (var error in errors)
                {
                    Console.WriteLine($"- {error}");
                }
            }
        );

        Console.WriteLine("");
        //result.Match(
        //    onSuccess: () => Results.NoContent(),
        //    onFailure: error => Results.BadRequest(error));

        #region Exemplo: Then, Tap, TapError, Catch, Finally

        Console.WriteLine("\n--- Exemplo Completo com Then, Tap, TapError, Catch, Finally ---\n");

        //var resultado = Result.Create<Usuario>()
        //    .Tap(() => Console.WriteLine("✓ Criando usuário..."))
        //    .Then(u => ValidarUsuario(u))
        //    .Tap(u => Console.WriteLine($"✓ Usuário '{u.Nome}' validado"))
        //    .TapError((errors, validationErrors) =>
        //    {
        //        if (errors.Any())
        //            Console.WriteLine($"✗ Erro geral: {string.Join(", ", errors)}");
        //        if (validationErrors.Any())
        //            Console.WriteLine($"✗ Erros de validação: {string.Join(", ", validationErrors.SelectMany(kv => kv.Value))}");
        //    })
        //    .Catch((errors, validationErrors, usuario) =>
        //    {
        //        Console.WriteLine("→ Tratando erro - Recuperando operação...");
        //        // Retorna um resultado de recuperação
        //        return Result.Ok(usuario);
        //    })
        //    .Finally(() => Console.WriteLine("✓ Operação finalizada\n"));

        #endregion




    }



    /// <summary>
    /// Valida um usuário e retorna Result<Usuario>.
    /// </summary>
    private static Result<Usuario> ValidarUsuario(Usuario usuario)
    {
        var resultado = Result.Ok(usuario);

        if (string.IsNullOrWhiteSpace(usuario.Nome))
            resultado.AddMessageError(nameof(usuario.Nome), "Nome é obrigatório");

        if (string.IsNullOrWhiteSpace(usuario.Email))
            resultado.AddMessageError(nameof(usuario.Email), "Email é obrigatório");

        if (string.IsNullOrWhiteSpace(usuario.Senha) && usuario.Senha?.Length < 6)
            resultado.AddMessageError(nameof(usuario.Senha), "Senha deve ter no mínimo 6 caracteres");

        return resultado;
    } 
    //    return result.Match(
    //        onSuccess: () => Ok(result.Value),
    //        onFailure: (errors, validationErrors) =>
    //        {
    //            if (validationErrors.Any())
    //                return BadRequest(result.ToValidationProblemDetails(HttpContext));

    //            return new { result.ToProblemDetails(HttpContext) };
    //        });
    //}
}