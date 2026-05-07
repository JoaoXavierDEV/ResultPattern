using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResultPattern.Validation;

namespace ResultPattern.Entity
{
    public class Produto
    {
        public Produto(string nome, decimal preco)
        {
            Nome = nome;
            Preco = preco;
        }
        public string Nome { get; private set; }
        public decimal Preco { get; private set; }
        public void AtualizarPreco(decimal novoPreco)
        {
            if (novoPreco <= 0)
                throw new ArgumentException("O preço deve ser maior que zero.", nameof(novoPreco));
            Preco = novoPreco;
        }

        public Result<Produto> Validar()
        {
            if (Preco <= 0)
                return Result.Fail<Produto>(x => x.Preco, ProdutoValidate.PrecoNegativo, this);
            return Result.Ok<Produto>(this);
        }

        public override string ToString()
        {
            return $"Produto: {Nome}, Preço: {Preco:C}";
        }
    }
}