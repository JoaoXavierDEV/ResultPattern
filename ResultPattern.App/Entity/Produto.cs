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

        public Result Validar()
        {
            if (Preco <= 0)
                return Result.Fail(ProdutoValidate.PrecoNegativo);
            return Result.Ok();
        }

        public override string ToString()
        {
            return $"Produto: {Nome}, Preço: {Preco:C}";
        }
    }
}