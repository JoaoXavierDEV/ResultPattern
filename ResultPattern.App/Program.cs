using ResultPattern;

public static class Program
{
    public static void Main(string[] args)
    {
        var user = new Usuario();

        var result = user.RegisterUser("test@example.com", "");

        var result2 = user.RegisterUser2("", "");


        if (result.IsSuccess)
        {
            Console.WriteLine("User registered successfully.");
        }
        else
        {
            Console.WriteLine($"Registration failed: {result.ToString()}");
        }
    }
}