using System.Globalization;
using ResultPattern;
using ResultPattern.Entity;
using ResultPattern.Service;

public static class Program
{
    public static void Main(string[] args)
    {
        var userService = new UsuarioService();

        var atual = CultureInfo.CurrentCulture;

        var user = new Usuario
        {
            Nome = "John Doe",
            Email = "dssdsd",
            Senha = "12345678"
        };

        // Retorna um resultado com sucesso ou falha
        var result = userService.RegisterUser(user);
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
            (x) =>
            {
                Console.WriteLine("User registered successfully.");
                //return 0;
            },
            (errors, x) =>
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




    }
}