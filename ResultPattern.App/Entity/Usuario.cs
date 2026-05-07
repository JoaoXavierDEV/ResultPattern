namespace ResultPattern.Entity;

public class Usuario : EntityBase
{
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Senha { get; set; }
    public Usuario()
    {
        Nome = string.Empty;
        Email = string.Empty;
        Senha = string.Empty;
    }
}

public class Pedido : EntityBase
{
    public string Codigo { get; set; }
    public List<Item> Itens { get; set; } = [];
}

public class Item : EntityBase
{
    public double Valor { get; set; }
    public double Quantidade { get; set; }
}

public abstract class EntityBase
{
    public long Id { get; set; }
}




