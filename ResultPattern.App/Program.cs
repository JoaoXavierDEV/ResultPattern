using System.Globalization;
using ResultPattern.Entity;
using ResultPattern.Service;

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





        Console.WriteLine("");
    }


}