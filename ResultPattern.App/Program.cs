using System.Globalization;
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




    }
}