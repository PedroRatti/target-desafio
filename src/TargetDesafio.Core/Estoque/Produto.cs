namespace TargetDesafio.Core.Estoque;

public sealed class Produto
{
    public int CodigoProduto { get; init; }
    public string DescricaoProduto { get; init; }
    public int Estoque { get; private set; }

    public Produto(int codigoProduto, string descricaoProduto, int estoque)
    {
        if (codigoProduto <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(codigoProduto), "O código do produto deve ser positivo.");
        }

        if (string.IsNullOrWhiteSpace(descricaoProduto))
        {
            throw new ArgumentException("A descrição do produto é obrigatória.", nameof(descricaoProduto));
        }

        if (estoque < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(estoque), "O estoque inicial não pode ser negativo.");
        }

        CodigoProduto = codigoProduto;
        DescricaoProduto = descricaoProduto.Trim();
        Estoque = estoque;
    }

    internal void AtualizarEstoque(int novoEstoque)
    {
        if (novoEstoque < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(novoEstoque), "O estoque não pode ficar negativo.");
        }

        Estoque = novoEstoque;
    }
}