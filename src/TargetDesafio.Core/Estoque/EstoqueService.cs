namespace TargetDesafio.Core.Estoque;

public sealed class EstoqueService
{
    private readonly Dictionary<int, Produto> _produtos;
    private readonly List<MovimentacaoEstoque> _movimentacoes = [];

    public EstoqueService(IEnumerable<Produto> produtos)
    {
        ArgumentNullException.ThrowIfNull(produtos);

        var lista = produtos.ToList();

        if (lista.Count == 0)
        {
            throw new ArgumentException("O estoque deve possuir pelo menos um produto.", nameof(produtos));
        }

        var possuiCodigoDuplicado = lista.GroupBy(produto => produto.CodigoProduto).Any(grupo => grupo.Count() > 1);

        if (possuiCodigoDuplicado)
        {
            throw new ArgumentException("Existem produtos com códigos duplicados.", nameof(produtos));
        }

        _produtos = lista.ToDictionary(produto => produto.CodigoProduto);
    }

    public IReadOnlyList<Produto> ListarProdutos()
    {
        return _produtos.Values.OrderBy(produto => produto.CodigoProduto).ToList();
    }

    public Produto BuscarProduto(int codigoProduto)
    {
        if (!_produtos.TryGetValue(codigoProduto, out var produto))
        {
            throw new KeyNotFoundException($"Produto de código {codigoProduto} não encontrado.");
        }

        return produto;
    }

    public IReadOnlyList<MovimentacaoEstoque> ListarMovimentacoes()
    {
        return _movimentacoes.ToList();
    }

    public MovimentacaoEstoque RegistrarMovimentacao(
        int codigoProduto,
        TipoMovimentacao tipo,
        int quantidade,
        string descricao
    )
    {
        if (quantidade <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantidade), "A quantidade movimentada deve ser positiva.");
        }

        if (string.IsNullOrWhiteSpace(descricao))
        {
            throw new ArgumentException("A descrição da movimentação é obrigatória.", nameof(descricao));
        }

        var produto = BuscarProduto(codigoProduto);
        var estoqueAnterior = produto.Estoque;

        var estoqueFinal = tipo switch
        {
            TipoMovimentacao.Entrada => checked(estoqueAnterior + quantidade),

            TipoMovimentacao.Saida
                when quantidade <= estoqueAnterior => estoqueAnterior - quantidade,

            TipoMovimentacao.Saida =>
                throw new InvalidOperationException($"Estoque insuficiente. Quantidade disponível: {estoqueAnterior}."),

            _ => throw new ArgumentOutOfRangeException(nameof(tipo), "Tipo de movimentação inválido.")
        };

        produto.AtualizarEstoque(estoqueFinal);

        var movimentacao = new MovimentacaoEstoque(
            Identificador: Guid.NewGuid(),
            CodigoProduto: produto.CodigoProduto,
            DescricaoProduto: produto.DescricaoProduto,
            Tipo: tipo,
            Descricao: descricao.Trim(),
            Quantidade: quantidade,
            EstoqueAnterior: estoqueAnterior,
            EstoqueFinal: estoqueFinal,
            RegistradaEm: DateTimeOffset.Now
        );

        _movimentacoes.Add(movimentacao);

        return movimentacao;
    }
}