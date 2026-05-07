namespace ResultPattern.Entity;

public class Usuario : EntityBase
{
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Senha { get; set; }
    public List<Pedido> Pedidos { get; set; } = [];
    public Endereco Endereco { get; set; } = new Endereco();
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

public class Endereco : EntityBase
{
    public string Rua { get; set; }
    public string Numero { get; set; }
    public string Cidade { get; set; }
    public string Estado { get; set; }
    public string Cep { get; set; }
}

public abstract class EntityBase
{
    public long Id { get; set; }
}




